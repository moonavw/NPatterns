using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPatterns.ObjectRelational;
using NPatterns.ObjectRelational.DynamicQuery;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NPatterns.Tests
{
    [TestClass]
    public class QueryObjectTest
    {
        [TestMethod]
        public void SerializeCriteria()
        {
            var criteria = new Criteria { Field = "Version", Operator = CriteriaOperator.IsEqualTo, Value = 1.0 };
            var jsonString = JsonConvert.SerializeObject(criteria, new StringEnumConverter());
            var criteria2 = JsonConvert.DeserializeObject<Criteria>(jsonString, new StringEnumConverter());

            Assert.AreEqual(criteria.Field, criteria2.Field);
            Assert.AreEqual(criteria.Operator, criteria2.Operator);
            Assert.AreEqual(criteria.Value, criteria2.Value);
        }

        [TestMethod]
        public void SerializeCriteriaGroup()
        {
            var criteriaGroup = new CriteriaGroup { Operator = CriteriaGroupOperator.Or };
            criteriaGroup.Criterias.Add(new Criteria { Field = "Version", Operator = CriteriaOperator.IsEqualTo, Value = 1.0 });
            criteriaGroup.Criterias.Add(new Criteria { Field = "Name", Operator = CriteriaOperator.Contains, Value = "NPatterns" });

            var jsonString = JsonConvert.SerializeObject(criteriaGroup, new StringEnumConverter());
            var criteriaGroup2 = JsonConvert.DeserializeObject<CriteriaGroup>(jsonString, new StringEnumConverter());

            Assert.AreEqual(criteriaGroup.Operator, criteriaGroup2.Operator);
            Assert.AreEqual(criteriaGroup.Criterias.Count, criteriaGroup2.Criterias.Count);
            for (int i = 0; i < criteriaGroup.Criterias.Count; i++)
            {
                var criteria = criteriaGroup.Criterias[i];
                var criteria2 = criteriaGroup2.Criterias[i];
                Assert.AreEqual(criteria.Field, criteria2.Field);
                Assert.AreEqual(criteria.Operator, criteria2.Operator);
                Assert.AreEqual(criteria.Value, criteria2.Value);
            }
        }

        [TestMethod]
        public void ExecuteDynamicQueryObject()
        {
            var source = new List<Product>
                             {
                                 new Product {Name = "NPatterns", Version = 1.2},
                                 new Product {Name = "NPatterns.Messaging.IoC",Version = 1.1},
                                 new Product {Name = "NPatterns.ObjectRelational.EF",Version = 1.0},
                                 new Product {Name = null,Version = 0}
                             };

            var query = new QueryObject();
            query.Add(new Criteria { Field = "Name", Operator = CriteriaOperator.IsNotNull });

            var criteriaGroup = new CriteriaGroup { Operator = CriteriaGroupOperator.Or };
            criteriaGroup.Criterias.Add(new Criteria { Field = "Name", Operator = CriteriaOperator.IsEqualTo, Value = "npatterns" });
            criteriaGroup.Criterias.Add(new Criteria { Field = "Name", Operator = CriteriaOperator.EndsWith, Value = "ioc" });

            query.Add(criteriaGroup);
            query.Add(new SortDescription { Field = "Version", Direction = SortDirection.Ascending });

            var executor = new DynamicQueryObjectExecutor();
            var result = query.Execute(source.AsQueryable(), executor).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].Version < result[1].Version);

            query.Add(new Criteria { Field = "Version", Operator = CriteriaOperator.IsEqualTo, Value = 1.0 }, CriteriaGroupOperator.Or);
            var result2 = query.Execute(source.AsQueryable(), executor).ToList();
            Assert.AreEqual(3, result2.Count);
        }

        [TestMethod]
        public void ExecuteDynamicQueryObjectWithComplexType()
        {
            var source = new List<ProductFeature>
                             {
                                 new ProductFeature
                                     {
                                         Product = new Product {Name = "NPatterns", Version = 1.2},
                                         Feature = new Feature {Name = "IUnitOfWork"}
                                     },
                                 new ProductFeature
                                     {
                                         Product = new Product {Name = "NPatterns.Messaging.IoC", Version = 1.1},
                                         Feature = new Feature {Name = "MessageBus"}
                                     },
                                 new ProductFeature
                                     {
                                         Product = new Product {Name = "NPatterns.ObjectRelational.EF", Version = 1.0},
                                         Feature = new Feature {Name = "UnitOfWork"}
                                     },
                                 new ProductFeature
                                     {
                                         Product = new Product(),
                                         Feature = new Feature()
                                     }
                             };

            var query = new QueryObject();
            query.Add(new Criteria { Field = "Product.Name", Operator = CriteriaOperator.IsNotNull });

            var criteriaGroup = new CriteriaGroup { Operator = CriteriaGroupOperator.Or };
            criteriaGroup.Criterias.Add(new Criteria { Field = "Product.Name", Operator = CriteriaOperator.IsEqualTo, Value = "npatterns" });
            criteriaGroup.Criterias.Add(new Criteria { Field = "Product.Name", Operator = CriteriaOperator.EndsWith, Value = "ioc" });

            query.Add(criteriaGroup);
            var executor = new DynamicQueryObjectExecutor();
            var result = query.Execute(source.AsQueryable(), executor).ToList();
            Assert.AreEqual(2, result.Count);

            query.Add(new Criteria { Field = "Product.Version", Operator = CriteriaOperator.IsEqualTo, Value = 1.0 }, CriteriaGroupOperator.Or);
            var result2 = query.Execute(source.AsQueryable(), executor).ToList();
            Assert.AreEqual(3, result2.Count);
        }

        public class Product
        {
            public string Name { get; set; }
            public double Version { get; set; }
        }

        public class Feature
        {
            public string Name { get; set; }
        }

        public class ProductFeature
        {
            public Product Product { get; set; }
            public Feature Feature { get; set; }
        }
    }
}
