using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Cci.Mappings;

namespace Microsoft.Cci.Differs
{
    public class ElementDifferenceFactory : IElementDifferenceFactory
    {
        private CompositionContainer _container;
        private IDifferenceRule[] _diffRules;
        private Func<IDifferenceRuleMetadata, bool> _ruleFilter;

        public ElementDifferenceFactory()
        {
        }

        public ElementDifferenceFactory(CompositionContainer container, Func<IDifferenceRuleMetadata, bool> ruleFilter = null)
        {
            Contract.Requires(container != null);
            _container = container;
            _ruleFilter = ruleFilter;
        }

        public IDifferences GetDiffer<T>(ElementMapping<T> element) where T : class
        {
            return new ElementDiffer<T>(element, GetDifferenceRules<T>());
        }

        private IDifferenceRule[] GetDifferenceRules<T>() where T : class
        {
            EnsureContainer();

            if (_diffRules == null)
            {
                IEnumerable<Lazy<IDifferenceRule, IDifferenceRuleMetadata>> lazyRules = _container.GetExports<IDifferenceRule, IDifferenceRuleMetadata>();
                if (_ruleFilter != null)
                {
                    lazyRules = lazyRules.Where(l => _ruleFilter(l.Metadata));
                }
                _diffRules = lazyRules.Select(l => l.Value).ToArray();
            }

            return _diffRules;
        }

        private void EnsureContainer()
        {
            if (_container != null)
                return;

            _container = new CompositionContainer(new AssemblyCatalog(typeof(ElementDifferenceFactory).Assembly));
        }
    }
}
