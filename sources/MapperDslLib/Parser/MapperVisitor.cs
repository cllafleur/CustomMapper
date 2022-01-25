namespace MapperDslLib.Parser
{
    using Antlr4.Runtime.Misc;
    using System.Collections.Generic;

    public class MapperVisitor : MapperBaseVisitor<object>
    {
        public override object VisitStatement([NotNull] MapperParser.StatementContext context)
        {
            var originExpr = (IExpressionMapper)this.Visit(context.extractExpr());
            IExpressionMapper targetExpr = null;
            targetExpr = (IExpressionMapper)this.Visit(context.insertExpr());
            return new StatementMapper(originExpr, targetExpr, new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitFunction([NotNull] MapperParser.FunctionContext context)
        {
            var identifier = context.IDENTIFIER().GetText();
            var arguments = new List<IExpressionMapper>();
            foreach (var child in context.children)
            {
                var arg = this.Visit(child);
                if (arg is IExpressionMapper expression)
                {
                    arguments.Add(expression);
                }
            }
            return new FunctionMapper(identifier, arguments, new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitTupleOfExpr([NotNull] MapperParser.TupleOfExprContext context)
        {
            List<IExpressionMapper> items = new List<IExpressionMapper>();
            foreach (var child in context.children)
            {
                var part = this.Visit(child);
                if (part is IExpressionMapper expression)
                {
                    items.Add(expression);
                }
            }
            return new TupleMapper(items.ToArray(), new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitNamedExpr([NotNull] MapperParser.NamedExprContext context)
        {
            var result = base.VisitNamedExpr(context);
            if (result is INamedExpressionMapper namedExpression)
            {
                namedExpression.ExpressionName = context.IDENTIFIER()?.GetText();
            }
            return result;
        }

        public override object VisitExpr([NotNull] MapperParser.ExprContext context)
        {
            if (context.LITTERAL() != null)
            {
                return new TextMapper(context.LITTERAL().GetText().Replace("\"", ""), new ParsingInfo(context.Start.Line, context.GetText()));
            }

            if (context.ChildCount == 0)
            {
                return null;
            }
            return this.Visit(context.GetChild(0));
        }

        public override object VisitInsertFieldRefStartNamed([NotNull] MapperParser.InsertFieldRefStartNamedContext context)
        {
            var instanceRefToken = context.insertInstanceRef();

            var instanceRef = instanceRefToken != null ? (InstanceRefMapper)this.Visit(instanceRefToken) : null;
            var setFieldRef = (FieldInstanceRefMapper)this.Visit(context.fieldOrArrayInstanceRef());

            return new InsertInstanceRefMapper(instanceRef, setFieldRef, new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitInsertFieldRefStartUnamed([NotNull] MapperParser.InsertFieldRefStartUnamedContext context)
        {
            var rootArray = (FieldInstanceRefMapper)this.Visit(context.startingUnamedArrayFieldInstanceRef());
            var instanceRefToken = context.insertInstanceRef();
            var setFieldToken = context.insertInstanceRef();

            var parsingInfos = new ParsingInfo(context.Start.Line, context.GetText());
            if (setFieldToken == null)
            {
                return new InsertInstanceRefMapper(null, rootArray, parsingInfos);
            }
            var setField = (FieldInstanceRefMapper)this.Visit(setFieldToken);
            if (instanceRefToken == null)
            {
                return new InsertInstanceRefMapper(new InstanceRefMapper(new[] { rootArray }, rootArray.ParsingInfo), setField, parsingInfos);
            }
            var instanceref = (InstanceRefMapper)this.Visit(instanceRefToken);
            var newInstanceRef = new InstanceRefMapper(new[] { rootArray }.Concat(instanceref.Children), instanceref.ParsingInfo);
            return new InsertInstanceRefMapper(newInstanceRef, setField, parsingInfos);
        }

        public override object VisitInsertInstanceRef([NotNull] MapperParser.InsertInstanceRefContext context)
        {
            var fieldsRef = new List<FieldInstanceRefMapper>();
            foreach (var field in context.children)
            {
                var part = this.Visit(field);
                if (part is FieldInstanceRefMapper fieldInstanceRefMapper)
                {
                    fieldsRef.Add(fieldInstanceRefMapper);
                }
            }
            return new InstanceRefMapper(fieldsRef, new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitInstanceRef([NotNull] MapperParser.InstanceRefContext context)
        {
            var fieldsRef = new List<FieldInstanceRefMapper>();
            foreach (var field in context.children)
            {
                var part = this.Visit(field);
                if (part is FieldInstanceRefMapper fieldInstanceRefMapper)
                {
                    fieldsRef.Add(fieldInstanceRefMapper);
                }
            }
            return new InstanceRefMapper(fieldsRef, new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitFieldInstanceRef([NotNull] MapperParser.FieldInstanceRefContext context)
        {
            return new FieldInstanceRefMapper(context.GetText(), new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitArrayFieldInstanceRef([NotNull] MapperParser.ArrayFieldInstanceRefContext context)
        {
            return new ArrayFieldInstanceRefMapper(context.fieldInstanceRef().GetText(), new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitStartingUnamedArrayFieldInstanceRef([NotNull] MapperParser.StartingUnamedArrayFieldInstanceRefContext context)
        {
            return new ArrayFieldInstanceRefMapper(String.Empty, new ParsingInfo(context.Start.Line, context.GetText()));
        }

        public override object VisitReturnFunctionDereferencement([NotNull] MapperParser.ReturnFunctionDereferencementContext context)
        {
            var function = this.Visit(context.function()) as FunctionMapper;
            var expressionTail = this.Visit(context.instanceRef()) as InstanceRefMapper;
            if (function != null && expressionTail != null)
            {
                return new ReturnFunctionExpressionMapper(function, expressionTail, new ParsingInfo(context.Start.Line, context.GetText()));
            }
            return null;
        }
    }
}
