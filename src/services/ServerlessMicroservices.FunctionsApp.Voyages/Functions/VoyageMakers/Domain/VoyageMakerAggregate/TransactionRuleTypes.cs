namespace ServerlessMicroservices.FunctionsApp.Voyages.Core.VoyageMaker.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class TransactionRuleTypes
    {
        public static readonly TransactionRuleTypes PointBasedRule = new TransactionRuleTypes(1, nameof(PointBasedRule));
        public static readonly TransactionRuleTypes BuyNGetNBasedRule = new TransactionRuleTypes(2, nameof(BuyNGetNBasedRule));

        public readonly int Id;
        public readonly string Name;

        private TransactionRuleTypes(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static IEnumerable<TransactionRuleTypes> List() => new[] { PointBasedRule, BuyNGetNBasedRule };

        public static TransactionRuleTypes FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for RuleType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TransactionRuleTypes From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for RuleType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}