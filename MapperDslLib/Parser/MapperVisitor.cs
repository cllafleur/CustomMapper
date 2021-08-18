namespace MapperDslLib.Parser
{
    using Antlr4.Runtime.Misc;
    using System.Collections.Generic;

    public class MapperVisitor : MapperBaseVisitor<object>
    {
        public override object VisitStatement([NotNull] MapperParser.StatementContext context)
        {
            var exprs = context.expr();
            var originExpr = (IExpressionMapper)this.Visit(exprs[0]);
            IExpressionMapper targetExpr = null;
            if (exprs.Length > 1)
            {
                targetExpr = (IExpressionMapper)this.Visit(exprs[1]);
            }
            return new StatementMapper(originExpr, targetExpr);
        }

        public override object VisitFunction([NotNull] MapperParser.FunctionContext context)
        {
            var identifier = context.IDENTIFIER().GetText();
            //var arguments = new[] { (IExpressionMapper)this.VisitExpr(context.expr()) };
            var arguments = new List<IExpressionMapper>();
            foreach (var child in context.children)
            {
                var arg = this.Visit(child);
                if (arg is IExpressionMapper expression)
                {
                    arguments.Add(expression);
                }
            }
            return new FunctionMapper(identifier, arguments);
        }

        public override object VisitExpr([NotNull] MapperParser.ExprContext context)
        {
            if (context.LITTERAL() != null)
            {
                return new TextMapper(context.LITTERAL().GetText().Replace("\"", ""));
            }

            if (context.ChildCount == 0)
            {
                return null;
            }
            return this.Visit(context.GetChild(0));
        }

        public override object VisitInstanceRef([NotNull] MapperParser.InstanceRefContext context)
        {
            return new InstanceRefMapper(context.GetText());
        }
    }
}
