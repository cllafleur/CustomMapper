using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib
{
    public class MapperVisitor : MapperBaseVisitor<object>
    {
        public override object VisitStatement([NotNull] MapperParser.StatementContext context)
        {
            var originExpr = (IExpressionMapper)this.Visit(context.expr(0));
            var targetExpr = (IExpressionMapper)this.Visit(context.expr(1));
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
                return new TextMapper(context.LITTERAL().GetText());
            }

            return this.Visit(context.GetChild(0));
        }

        public override object VisitInstanceRef([NotNull] MapperParser.InstanceRefContext context)
        {
            return new InstanceRefMapper(context.GetText());
        }
    }
}
