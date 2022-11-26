using ArithmeticString.Compiling.Interfaces;
using ArithmeticString.Compiling.Nodes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArithmeticString.Compiling
{
    public class Compiler<T>
    {
        IParser<T> _parser;
        Ruler<T> _ruler;
        Dictionary<string, T> _constants;

        HashSet<char> _prefixs;
        Dictionary<char, IDecorator<T>> _prefixDecorators;
        Dictionary<char, IDecorator<T>> _suffixDecorators;

        Dictionary<string, Func<T[], T>> _funcs;

        #region -- Constructor --
        public Compiler(IParser<T> parser)
        {
            _parser = parser;
            _ruler = new Ruler<T>();
            _constants = new Dictionary<string, T>();
            _prefixs = new HashSet<char>();
            _prefixDecorators = new Dictionary<char, IDecorator<T>>();
            _suffixDecorators = new Dictionary<char, IDecorator<T>>();
            _funcs = new Dictionary<string, Func<T[], T>>();
        }
        #endregion

        #region -- Install --
        public Compiler<T> InstallOperator<TOperator>(string symbol, int priority = 0) where TOperator : IOperator<T>
        {
            var @operator = Activator.CreateInstance<TOperator>();
            _ruler.Add(symbol, @operator, priority);
            return this;
        }

        public Compiler<T> InstallPrefix<TDecorator>(char prefix) where TDecorator : IDecorator<T>
        {
            var decorator = Activator.CreateInstance<TDecorator>();
            _prefixs.Add(prefix);
            _prefixDecorators.Add(prefix, decorator);
            return this;
        }

        public Compiler<T> InstallFunc(string name, Func<T[], T> func)
        {
            _funcs.Add(name, func);
            return this;
        }
        #endregion

        public Compiler<T> SetConstant(string name, T value)
        {
            if (!_constants.ContainsKey(name))
            {
                _constants.Add(name, value);
                return this;
            }
            _constants[name] = value;
            return this;
        }

        public Compiler<T> SetConstant(T value, params string[] names)
        {
            foreach (var name in names)
            {
                SetConstant(name, value);
                return this;
            }
            return this;
        }

        #region -- Add --
        private Node<T> addConstValue(GroupNode<T> group, string seg)
        {
            var node = new ValueNode<T>(_parser.Parse(seg));
            group.Add(node);
            return node;
        }

        private Node<T> addConstValue(GroupNode<T> group, T value)
        {
            var node = new ValueNode<T>(value);
            group.Add(node);
            return node;
        }

        private Node<T> addVariableValue(GroupNode<T> group, string seg)
        {
            var node = new VariableNode<T>(seg);
            group.Add(node);
            return node;
        }


        private Node<T> addValuableNode(GroupNode<T> current, ISet<string> parameters, string seg)
        {
            if (seg.Length < 1)
                return null;
            Node<T> node;
            if (isNumber(seg))
                node = addConstValue(current, seg);
            else if (isConst(seg))
                node = addConstValue(current, _constants[seg]);
            else
            {
                parameters.Add(seg);
                node = addVariableValue(current, seg);
            }
            return node;
        }

        private Node<T> addOperatorNode(GroupNode<T> current, string symbol)
        {
            var @operator = _ruler[symbol];
            var opNode = (OperatorNode<T>)Activator.CreateInstance(typeof(OperatorNode<T>), @operator, symbol, _ruler.GetPriority(symbol));

            current.Add(opNode);
            return opNode;
        }
        #endregion

        #region -- Detect Node --
        private bool isOperatorNode(Node<T> node)
        {
            if (node == null)
                return false;
            var nodeType = node.GetType();
            var operatorType = typeof(OperatorNode<T>);
            return nodeType == operatorType || nodeType.IsSubclassOf(operatorType);
        }


        private bool isFuncNode(GroupNode<T> node)
        {
            if (node == null)
                return false;
            var type = node.GetType();
            var funcType = typeof(FuncNode<T>);
            return type == funcType || type.IsSubclassOf(funcType);
        }

        private bool isPreffixNode(GroupNode<T> current)
        {
            var type = current.GetType();
            var preffixType = typeof(PrefixDecoratorNode<T>);
            return type == preffixType || type.IsSubclassOf(preffixType);
        }
        #endregion

        #region -- Detect Character --
        private bool isEqualSign(char c)
        {
            return c == '=';
        }

        private bool isPreffix(char c)
        {
            return _prefixs.Contains(c);
        }


        private bool isConst(string seg)
        {
            return _constants.ContainsKey(seg);
        }


        private bool isCharactor(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }


        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9' || c == '.';
        }

        private bool isSpace(char c)
        {
            return c == ' ';
        }

        private bool isComma(char c)
        {
            return c == ',';
        }

        private bool isOperator(string seg)
        {
            if (seg.Length < 1)
                return false;
            return _ruler.ContainsSymbol(seg);
        }

        private bool isNumber(string seg)
        {
            if (seg.Length < 1)
                return false;
            foreach (var c in seg)
            {
                if (!isDigit(c))
                    return false;
            }
            return true;
        }

        private bool isLQuota(char c)
        {
            return c == '(';
        }

        private bool isRQuota(char c)
        {
            return c == ')';
        }

        private bool isSemicolon(char c)
        {
            return c == ';';
        }

        private bool isNewLine(char c)
        {
            return c == '\n';
        }
        #endregion

        #region -- Group -- 
        private (GroupNode<T>, Node<T>) startFunc(Stack<GroupNode<T>> stack, GroupNode<T> current, string seg)
        {
            var funcName = seg;
            _funcs.TryGetValue(funcName, out Func<T[], T> func);
            stack.Push(current);
            stack.Push(new FuncNode<T>(funcName, func));
            return (new ExprNode<T>(), null);
        }

        private bool isInFunc(GroupNode<T> current)
        {
            var type = current.GetType();
            var exprType = typeof(ExprNode<T>);
            return type == exprType || type.IsSubclassOf(exprType);
        }

        private (GroupNode<T>, Node<T>) endFunc(Stack<GroupNode<T>> stack, GroupNode<T> current)
        {
            // quit inner expr
            (current, _) = endQuota(stack, current);
            // quit func
            var (recent, _) = endQuota(stack, current);
            return (recent, recent);
        }

        private (GroupNode<T>, Node<T>) startQuota(Stack<GroupNode<T>> stack, GroupNode<T> current)
        {
            stack.Push(current);
            return (new QuotaNode<T>(), null);
        }

        private (GroupNode<T>, Node<T>) endQuota(Stack<GroupNode<T>> stack, GroupNode<T> current)
        {
            var recent = stack.Pop();
            current.Parent = recent;
            recent.Add(current);
            return (recent, recent);
        }

        private (GroupNode<T>, Node<T>) startPreffixDecorator(Stack<GroupNode<T>> stack, GroupNode<T> current, char c)
        {
            var decorator = _prefixDecorators[c];
            stack.Push(current);
            return (new PrefixDecoratorNode<T>(decorator, c), null);
        }

        private (GroupNode<T>, Node<T>) endPreffixDecorator(Stack<GroupNode<T>> stack, GroupNode<T> current)
        {
            (current, _) = endQuota(stack, current);
            return (current, current);
        }
        #endregion

        #region -- Parse --
        public Equation<T> Parse(Stream stream, CancellationTokenSource cts = null)
        {
            GroupNode<T> root = new ExprNode<T>();
            Stack<GroupNode<T>> stack = new Stack<GroupNode<T>>();
            var eqName = string.Empty;
            string seg = string.Empty;
            var current = root;
            HashSet<string> parameters = new HashSet<string>();
            Node<T> _last = null;
            var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                if (cts != null && cts.IsCancellationRequested)
                    throw new TaskCanceledException();
                var c = (char)reader.Read();
                if (isEqualSign(c))
                {
                    while (stack.Count > 0)
                        (current, _) = endQuota(stack, current);
                    if (root.Root.GetType() == typeof(FuncNode<T>))
                    {
                        eqName = ((FuncNode<T>)root.Root).Name;
                    }
                    else if (root.Root.GetType() == typeof(VariableNode<T>))
                    {
                        eqName = ((VariableNode<T>)root.Root).Name;
                    }
                    else
                    {
                        // random the equation name with length 6
                        eqName = RandomString(6);
                    }
                    // reset expr
                    seg = string.Empty;
                    root = new ExprNode<T>();
                    stack.Clear();
                    parameters.Clear();
                    _last = null;
                    current = root = new ExprNode<T>();
                    continue;

                }
                if (isSemicolon(c))
                {
                    break;
                }
                // space
                if (isSpace(c) || isNewLine(c))
                {
                    if (seg.Length < 1)
                        continue;
                    if (isOperator(seg))
                        _last = addOperatorNode(current, seg);
                    else
                    {
                        _last = addValuableNode(current, parameters, seg);
                        // end of prefix
                        if (isPreffixNode(current))
                            (current, _last) = endPreffixDecorator(stack, current);
                    }
                    seg = string.Empty;
                    continue;
                }
                // comma
                if (isComma(c))
                {
                    _last = addValuableNode(current, parameters, seg);
                    // end of prefix
                    if (isPreffixNode(current))
                    {
                        (current, _last) = endPreffixDecorator(stack, current);
                    }
                    (current, _) = endQuota(stack, current);
                    stack.Push(current);
                    _last = null;
                    current = new ExprNode<T>();
                    seg = string.Empty;
                    continue;
                }
                // prefix
                if (isPreffix(c) && (_last == null || isOperatorNode(_last)))
                {
                    /*
                     * if expr is "1-2", then the seg will be not empty
                     * else if "1+-2", the seg will be empty & last current node is operator
                     */
                    (current, _last) = startPreffixDecorator(stack, current, c);
                    seg = string.Empty;
                    continue;
                }
                // operator / digit
                if (isDigit(c) && isOperator(seg))
                {
                    _last = addOperatorNode(current, seg);
                    seg = string.Empty;
                }
                if (!isDigit(c))
                {
                    if (isNumber(seg))
                    {
                        _last = addConstValue(current, seg);
                        // end of prefix
                        if (isPreffixNode(current))
                        {
                            (current, _last) = endPreffixDecorator(stack, current);
                        }
                        seg = string.Empty;
                    }
                    if (isConst(seg))
                    {
                        // add const
                        _last = addConstValue(current, _constants[seg]);
                        // end of prefix
                        if (isPreffixNode(current))
                        {
                            (current, _last) = endPreffixDecorator(stack, current);
                        }
                        seg = string.Empty;
                    }
                }
                // quota
                if (isLQuota(c))
                {
                    if (isOperator(seg))
                    {
                        _last = addOperatorNode(current, seg);
                        (current, _) = startQuota(stack, current);
                    }
                    else
                    {
                        if (seg.Length > 0)
                            (current, _last) = startFunc(stack, current, seg);
                        else
                            (current, _last) = startQuota(stack, current);
                    }
                    seg = string.Empty;
                    continue;
                }
                if (isRQuota(c))
                {
                    _last = addValuableNode(current, parameters, seg);
                    // end of prefix
                    if (isPreffixNode(current))
                        (current, _last) = endPreffixDecorator(stack, current);
                    // end of func
                    if (isInFunc(current))
                        (current, _last) = endFunc(stack, current);
                    else // end of quota
                        (current, _last) = endQuota(stack, current);
                    // end of prefix
                    if (isPreffixNode(current))
                        (current, _last) = endPreffixDecorator(stack, current);
                    seg = string.Empty;
                    continue;
                }

                seg += c;
            }
            // handle last block
            _last = addValuableNode(current, parameters, seg);
            if (isPreffixNode(current))
                (_, _last) = endPreffixDecorator(stack, current);
            // quit all stack except root quota
            while (stack.Count > 1)
                stack.Pop();
            return new Equation<T>(eqName, root, parameters.ToArray());
        }

        public Equation<T> Parse(string formula, CancellationTokenSource cts = null)
        {
            var stream = GetStream(formula);
            try
            {
                return Parse(stream, cts);
            }
            finally
            {
                RecycleStream(stream);
            }
        }

        public Task<Equation<T>> ParseAsync(Stream stream, CancellationTokenSource cts = null)
        {
            var tcs = new TaskCompletionSource<Equation<T>>();
            Task.Run(() =>
            {
                try
                {
                    GroupNode<T> root = new ExprNode<T>();
                    Stack<GroupNode<T>> stack = new Stack<GroupNode<T>>();
                    var eqName = string.Empty;
                    string seg = string.Empty;
                    var current = root;
                    HashSet<string> parameters = new HashSet<string>();
                    Node<T> _last = null;
                    var reader = new StreamReader(stream);
                    while (!reader.EndOfStream)
                    {
                        if (cts != null && cts.IsCancellationRequested)
                            throw new TaskCanceledException();
                        var c = (char)reader.Read();
                        if (isEqualSign(c))
                        {
                            while (stack.Count > 0)
                                (current, _) = endQuota(stack, current);
                            if (root.Root.GetType() == typeof(FuncNode<T>))
                            {
                                eqName = ((FuncNode<T>)root.Root).Name;
                            }
                            else if (root.Root.GetType() == typeof(VariableNode<T>))
                            {
                                eqName = ((VariableNode<T>)root.Root).Name;
                            }
                            else
                            {
                                // random the equation name with length 6
                                eqName = RandomString(6);
                            }
                            // reset expr
                            seg = string.Empty;
                            root = new ExprNode<T>();
                            stack.Clear();
                            parameters.Clear();
                            _last = null;
                            current = root = new ExprNode<T>();
                            continue;

                        }
                        if (isSemicolon(c))
                        {
                            break;
                        }
                        // space
                        if (isSpace(c) || isNewLine(c))
                        {
                            if (seg.Length < 1)
                                continue;
                            if (isOperator(seg))
                                _last = addOperatorNode(current, seg);
                            else
                            {
                                _last = addValuableNode(current, parameters, seg);
                                // end of prefix
                                if (isPreffixNode(current))
                                    (current, _last) = endPreffixDecorator(stack, current);
                            }
                            seg = string.Empty;
                            continue;
                        }
                        // comma
                        if (isComma(c))
                        {
                            _last = addValuableNode(current, parameters, seg);
                            // end of prefix
                            if (isPreffixNode(current))
                            {
                                (current, _last) = endPreffixDecorator(stack, current);
                            }
                            (current, _) = endQuota(stack, current);
                            stack.Push(current);
                            _last = null;
                            current = new ExprNode<T>();
                            seg = string.Empty;
                            continue;
                        }
                        // prefix
                        if (isPreffix(c) && (_last == null || isOperatorNode(_last)))
                        {
                            /*
                             * if expr is "1-2", then the seg will be not empty
                             * else if "1+-2", the seg will be empty & last current node is operator
                             */
                            (current, _last) = startPreffixDecorator(stack, current, c);
                            seg = string.Empty;
                            continue;
                        }
                        // operator / digit
                        if (isDigit(c) && isOperator(seg))
                        {
                            _last = addOperatorNode(current, seg);
                            seg = string.Empty;
                        }
                        if (!isDigit(c))
                        {
                            if (isNumber(seg))
                            {
                                _last = addConstValue(current, seg);
                                // end of prefix
                                if (isPreffixNode(current))
                                {
                                    (current, _last) = endPreffixDecorator(stack, current);
                                }
                                seg = string.Empty;
                            }
                            if (isConst(seg))
                            {
                                // add const
                                _last = addConstValue(current, _constants[seg]);
                                // end of prefix
                                if (isPreffixNode(current))
                                {
                                    (current, _last) = endPreffixDecorator(stack, current);
                                }
                                seg = string.Empty;
                            }
                        }
                        // quota
                        if (isLQuota(c))
                        {
                            if (isOperator(seg))
                            {
                                _last = addOperatorNode(current, seg);
                                (current, _) = startQuota(stack, current);
                            }
                            else
                            {
                                if (seg.Length > 0)
                                    (current, _last) = startFunc(stack, current, seg);
                                else
                                    (current, _last) = startQuota(stack, current);
                            }
                            seg = string.Empty;
                            continue;
                        }
                        if (isRQuota(c))
                        {
                            _last = addValuableNode(current, parameters, seg);
                            // end of prefix
                            if (isPreffixNode(current))
                                (current, _last) = endPreffixDecorator(stack, current);
                            // end of func
                            if (isInFunc(current))
                                (current, _last) = endFunc(stack, current);
                            else // end of quota
                                (current, _last) = endQuota(stack, current);
                            // end of prefix
                            if (isPreffixNode(current))
                                (current, _last) = endPreffixDecorator(stack, current);
                            seg = string.Empty;
                            continue;
                        }

                        seg += c;
                    }
                    // handle last block
                    _last = addValuableNode(current, parameters, seg);
                    if (isPreffixNode(current))
                        (_, _last) = endPreffixDecorator(stack, current);
                    // quit all stack except root quota
                    while (stack.Count > 1)
                        stack.Pop();
                    tcs.SetResult(new Equation<T>(eqName, root, parameters.ToArray()));

                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }

        public Task<Equation<T>> ParseAsync(string formula, CancellationTokenSource cts = null)
        {
            var stream = GetStream(formula);
            var task = ParseAsync(stream, cts);
            task.ContinueWith((t) => RecycleStream(stream));
            return task;
        }
        #endregion

        #region -- Compile --
        public EquationCollection<T> Compile(Stream stream, CancellationTokenSource cts = null)
        {
            List<Equation<T>> eqs = new List<Equation<T>>();
            string eqStr = string.Empty;
            var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                if (cts != null && cts.IsCancellationRequested)
                    throw new TaskCanceledException();
                var c = (char)reader.Read();
                if (isSemicolon(c))
                {
                    var equation = Parse(eqStr);
                    eqs.Add(equation);
                    eqStr = string.Empty;
                    continue;
                }
                eqStr += c;
            }
            if (!string.IsNullOrEmpty(eqStr))
            {
                var equation = Parse(eqStr);
                eqs.Add(equation);
            }
            return new EquationCollection<T>(eqs.ToArray());
        }

        public EquationCollection<T> Compile(string script, CancellationTokenSource cts = null)
        {
            var stream = GetStream(script);
            try
            {
                return Compile(stream, cts);
            }
            finally
            {
                RecycleStream(stream);
            }
        }

        public Task<EquationCollection<T>> CompileAsync(Stream stream, CancellationTokenSource cts = null)
        {
            var src = new TaskCompletionSource<EquationCollection<T>>();
            Task.Run(() =>
            {
                try
                {
                    var eq = Compile(stream, cts);
                    src.SetResult(eq);
                }
                catch (TaskCanceledException)
                {
                    src.SetCanceled();
                }
                catch (Exception e)
                {
                    src.SetException(e);
                }
            });
            return src.Task;
        }

        public Task<EquationCollection<T>> CompileAsync(string script, CancellationTokenSource cts = null)
        {
            var stream = GetStream(script);
            var task = CompileAsync(stream, cts);
            task.ContinueWith((t) => RecycleStream(stream));
            return task;
        }
        #endregion

        private static string RandomString(int length)
        {
            var random = new Random(DateTime.UtcNow.Millisecond);
            const string chars = "abcdefghijklmnopqrstuvABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #region -- Inner Stream Pool --
        static ConcurrentBag<Stream> _streamPool = new ConcurrentBag<Stream>();

        private Stream GetStream(string text)
        {
            if (!_streamPool.TryTake(out Stream stream))
            {
                stream = new MemoryStream();
            }
            stream.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
            stream.Position = 0;
            stream.SetLength(text.Length);
            return stream;
        }

        private void RecycleStream(Stream stream)
        {
            stream.Position = 0;
            _streamPool.Add(stream);
        }
        #endregion
    }

    public class Ruler<T>
    {
        public HashSet<string> _symbols = new HashSet<string>();
        public Dictionary<string, IOperator<T>> _operators = new Dictionary<string, IOperator<T>>();
        public Dictionary<string, int> _priorities = new Dictionary<string, int>();
        public List<Dictionary<string, IOperator<T>>> _sortedOperators = new List<Dictionary<string, IOperator<T>>>();

        public IOperator<T> this[string key] => _operators[key];

        public void Add(string symbol, IOperator<T> @operator, int priority = 0)
        {
            if (priority < 0)
                throw new ArgumentException();
            while (_sortedOperators.Count < priority + 1)
            {
                _sortedOperators.Add(new Dictionary<string, IOperator<T>>());
            }
            _symbols.Add(symbol);
            _operators.Add(symbol, @operator);
            _priorities.Add(symbol, priority);
            _sortedOperators[priority].Add(symbol, @operator);
        }

        public int GetPriority(string symbol)
        {
            return _priorities[symbol];
        }

        public IReadOnlyDictionary<string, IOperator<T>> GetOperators(int priority = 0)
        {
            if (priority < 0)
                throw new ArgumentException();
            if (_sortedOperators.Count < priority + 1)
                return null;
            return _sortedOperators[priority];
        }

        public bool ContainsSymbol(string symbol)
        {
            return _symbols.Contains(symbol);
        }

        public IEnumerable<string[]> SymbolsByPriority()
        {
            string[][] array = new string[_sortedOperators.Count][];
            for (int i = 0; i < array.Length; i++)
            {
                int index = array.Length - i - 1;
                array[i] = _sortedOperators[index].Keys.ToArray();
            }
            return array;
        }
    }
}
