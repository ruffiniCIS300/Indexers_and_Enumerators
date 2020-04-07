/* DictionaryTests.cs
 * Author: Rod Howell
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Ksu.Cis300.NameLookup.Tests
{
    /// <summary>
    /// Unit tests for the Dictionary class.
    /// </summary>
    [TestFixture]
    public class DictionaryTests
    {
        /// <summary>
        /// Tests that looking up a nonexistent key using the indexer throws the correct exception.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestALookUpEmpty()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            Exception e = null;
            try
            {
                int v = d["abc"];
            }
            catch (Exception ex)
            {
                e = ex;
            }
            Assert.That(e, Is.Not.Null.And.TypeOf(typeof(KeyNotFoundException)));
        }

        /// <summary>
        /// Tests replacing a key's value.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestAReplaceKey()
        {
            Dictionary<int, string> d = new Dictionary<int, string>();
            d.Add(4, "four");
            d[4] = "again";
            bool lookup = d.TryGetValue(4, out string v);
            Assert.Multiple(() =>
            {
                Assert.That(lookup, Is.True);
                Assert.That(d.Count, Is.EqualTo(1));
                Assert.That(v, Is.EqualTo("again"));
            });
        }

        /// <summary>
        /// Adds a key and a value, then looks up that key using the indexer.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestBAddOneLookUpWithIndexer()
        {
            Dictionary<int, string> d = new Dictionary<int, string>();
            int k = 100000;
            d.Add(k, "value");
            Assert.That(d[k], Is.EqualTo("value"));
        }

        /// <summary>
        /// Adds a key and a value using the indexer, then looks up that key using TryGetValue.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestBAddOneWithIndexerTryGetValue()
        {
            Dictionary<int, string> d = new Dictionary<int, string>();
            int k = 100000;
            d[k] = "value";
            bool lookup = d.TryGetValue(k, out string v);
            Assert.Multiple(() =>
            {
                Assert.That(lookup, Is.True);
                Assert.That(d.Count, Is.EqualTo(1));
                Assert.That(v, Is.EqualTo("value"));
            });
        }

        /// <summary>
        /// Adds two keys that should be stored in the same list, then looks up the first (which
        /// should be second in the list) using the indexer.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestCAddTwoLookUpFirstWithIndexer()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k1 = new HashTableTester(1000);
            HashTableTester k2 = new HashTableTester(1025);
            d.Add(k1, "first");
            d.Add(k2, "second");
            Assert.That(d[k1], Is.EqualTo("first"));
        }

        /// <summary>
        /// Adds two keys that should be stored in the same list, then looks up the second using
        /// the indexer.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestCAddTwoLookUpSecondWithIndexer()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k1 = new HashTableTester(10000);
            HashTableTester k2 = new HashTableTester(10025);
            d.Add(k1, "first");
            d.Add(k2, "second");
            Assert.That(d[k2], Is.EqualTo("second"));
        }

        /// <summary>
        /// Adds two keys that should be stored in the same list using the indexer, then looks up the first (which
        /// should be second in the list) using TryGetValue.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestCAddTwoWithIndexerTryGetValueFirst()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k1 = new HashTableTester(1000);
            HashTableTester k2 = new HashTableTester(1025);
            d[k1] = "first";
            d[k2] = "second";
            d.TryGetValue(k1, out string v);
            Assert.Multiple(() =>
            {
                Assert.That(d.Count, Is.EqualTo(2));
                Assert.That(v, Is.EqualTo("first"));
            });
        }

        /// <summary>
        /// Adds two keys that should be stored in the same list using the indexer, then looks up the second
        /// using TryGetValue.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestCAddTwoWithIndexerTryGetValueFSecond()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k1 = new HashTableTester(1000);
            HashTableTester k2 = new HashTableTester(1025);
            d[k1] = "first";
            d[k2] = "second";
            d.TryGetValue(k2, out string v);
            Assert.That(v, Is.EqualTo("second"));
        }

        /// <summary>
        /// Test that two keys with hash codes that differ by 5 map to the same location.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestDTwoInstancesSameLocation()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            HashTableTester k1 = new HashTableTester(100, true);
            HashTableTester k2 = new HashTableTester(105, true);
            d[k1] = 7;
            // Because k1 and k2 are constructed to be equal to all other instances of HashTableTester, the dictionary
            // should find k2 if it maps to the same array location as k1.
            Assert.That(d.TryGetValue(k2, out int v), Is.True);
        }

        /// <summary>
        /// Adds 4 keys that should end up in different locations, then checks whether each
        /// of the 5 locations has a key.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestDDifferentLocations()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            List<int> elements = new List<int>(); // Will contain the values stored in the dictionary
            elements.Add(0); // Represents one hash table location that will remain empty.
            for (int i = 500; i < 504; i++)
            {
                d[new HashTableTester(i, true)] = i;
                elements.Add(i);
            }
            List<int> retrieved = new List<int>();

            // The following loop should check each of the table locations.
            // If the location contains an empty list, v will be set to 0.
            // Otherwise, v will be set to the value from the first key-value pair
            // in the list at that location. The first list checked should be
            // empty, and the others should contain the elements added in the above
            // loop, in the same order.
            for (int i = 504; i < 509; i++)
            {
                int v;
                d.TryGetValue(new HashTableTester(i, true), out v);
                retrieved.Add(v);
            }
            Assert.That(retrieved, Is.Ordered.And.EquivalentTo(elements));
        }

        /// <summary>
        /// Tests that adding 5 keys and values does not cause a rehash to occur.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestEAddFive()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            List<int> elements = new List<int>(); // Will contain the values stored in the dictionary
            for (int i = 199; i < 204; i++)
            {
                d[new HashTableTester(i)] = i;
                elements.Add(i);
            }
            List<int> retrieved = new List<int>();

            // The following loop should check each of the table locations.
            // If the location contains an empty list, v will be set to 0.
            // Otherwise, v will be set to the value from the first key-value pair
            // in the list at that location. The locations checked should contain 
            // the elements added in the above
            // loop, in the same order.
            for (int i = 204; i < 209; i++)
            {
                int v;
                d.TryGetValue(new HashTableTester(i, true), out v);
                retrieved.Add(v);
            }
            Assert.That(retrieved, Is.Ordered.And.EquivalentTo(elements));
        }

        /// <summary>
        /// Tests rehashing by adding 6 keys that should end up in different lists
        /// after the rehashing is done.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestFRehash()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            List<int> elements = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                elements.Add(0); // Represents 5 empty linked lists.
            }
            for (int i = 1; i < 7; i++)
            {
                d[new HashTableTester(i)] = i;
                elements.Add(i);
            }
            List<int> retrieved = new List<int>();

            // The following loop should check each of the table locations.
            // If the location contains an empty list, v will be set to 0.
            // Otherwise, v will be set to the value from the first key-value pair
            // in the list at that location. The locations checked should contain 
            // five empty lists, followed by the elements added in the above
            // loop, in the same order.
            for (int i = 7; i < 18; i++)
            {
                int v;
                d.TryGetValue(new HashTableTester(i, true), out v);
                retrieved.Add(v);
            }
            Assert.That(retrieved, Is.Ordered.And.EquivalentTo(elements));
        }

        /// <summary>
        /// Tests rehashing when all elements should end up in the same list after
        /// rehashing.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestGRehashToSameList()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            List<int> elements = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                elements.Add(0); // Reprensents 10 empty lists.
            }
            for (int i = 50; i < 116; i += 11)
            {
                d[new HashTableTester(i)] = i; // Adds 6 keys that should end up in the same list after rehashing
                elements.Add(i);
            }
            List<int> retrieved = new List<int>();
            int v;

            // The following loop checks 10 of the lists, each of which should be empty.
            // It should add ten 0s to retrieved.
            for (int i = 51; i < 61; i++)
            {
                d.TryGetValue(new HashTableTester(i, true), out v);
                retrieved.Add(v);
            }

            // The following loop looks up all the keys that were added and adds the retrieved
            // values to retrieved.
            for (int i = 50; i < 116; i += 11)
            {
                d.TryGetValue(new HashTableTester(i), out v);
                retrieved.Add(v);
            }
            Assert.That(retrieved, Is.Ordered.And.EquivalentTo(elements));
        }

        /// <summary>
        /// Tests rehashing twice such that all elements should end up in the same list after
        /// the second rehash.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestHDoubleRehashToSameList()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            List<int> elements = new List<int>();
            for (int i = 0; i < 22; i++)
            {
                elements.Add(0); // Reprensents 22 empty lists.
            }
            for (int i = 100; i < 376; i += 23)
            {
                d[new HashTableTester(i)] = i; // Adds 12 keys that should end up in the same list after rehashing twice
                elements.Add(i);
            }
            List<int> retrieved = new List<int>();
            int v;

            // The following loop checks 22 of the lists, each of which should be empty.
            // It should add 22 0s to retrieved.
            for (int i = 101; i < 123; i++)
            {
                d.TryGetValue(new HashTableTester(i, true), out v);
                retrieved.Add(v);
            }

            // The following loop looks up all the keys that were added and adds the retrieved
            // values to retrieved.
            for (int i = 100; i < 376; i += 23)
            {
                d.TryGetValue(new HashTableTester(i), out v);
                retrieved.Add(v);
            }
            Assert.That(retrieved, Is.Ordered.And.EquivalentTo(elements));
        }

        /// <summary>
        /// Tests that a foreach correctly iterates through an empty dictionary.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestIForeachEmpty()
        {
            Dictionary<string, double> d = new Dictionary<string, double>();
            List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
            foreach (KeyValuePair<string, double> p in d)
            {
                list.Add(p);
            }
            Assert.That(list, Is.Empty);
        }

        /// <summary>
        /// Tests a foreach on a dictionary whose hash table contains lists with 0-3 cells.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestJForeachNonempty()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            KeyValuePair<HashTableTester, int>[] pairs =
            {
                new KeyValuePair<HashTableTester, int>(new HashTableTester(50), 50),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(90), 90),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(39), 39),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(68), 68),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(17), 17),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(43), 43)
            };
            foreach (KeyValuePair<HashTableTester, int> p in pairs)
            {
                d.Add(p.Key, p.Value);
            }
            List<KeyValuePair<HashTableTester, int>> list = new List<KeyValuePair<HashTableTester, int>>();
            foreach (KeyValuePair<HashTableTester, int> p in d)
            {
                list.Add(p);
            }
            Assert.That(list, Is.EquivalentTo(pairs));
        }

        /// <summary>
        /// Tests a foreach iterating through the keys of a dictionary whose hash table contains lists with 0-3 cells.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestKForeachNonemptyKeys()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            KeyValuePair<HashTableTester, int>[] pairs =
            {
                new KeyValuePair<HashTableTester, int>(new HashTableTester(50), 50),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(90), 90),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(39), 39),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(68), 68),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(17), 17),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(43), 43)
            };
            List<HashTableTester> keys = new List<HashTableTester>();
            foreach (KeyValuePair<HashTableTester, int> p in pairs)
            {
                d.Add(p.Key, p.Value);
                keys.Add(p.Key);
            }
            List<HashTableTester> list = new List<HashTableTester>();
            foreach (HashTableTester k in d.Keys)
            {
                list.Add(k);
            }
            Assert.That(list, Is.EquivalentTo(keys));
        }

        /// <summary>
        /// Tests a foreach iterating through the values of a dictionary whose hash table contains lists with 0-3 cells.
        /// </summary>
        [Test, Timeout(1000)]
        public void TestKForeachNonemptyValues()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            KeyValuePair<HashTableTester, int>[] pairs =
            {
                new KeyValuePair<HashTableTester, int>(new HashTableTester(50), 50),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(90), 90),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(39), 39),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(68), 68),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(17), 17),
                new KeyValuePair<HashTableTester, int>(new HashTableTester(43), 43)
            };
            List<int> values = new List<int>();
            foreach (KeyValuePair<HashTableTester, int> p in pairs)
            {
                d.Add(p.Key, p.Value);
                values.Add(p.Value);
            }
            List<int> list = new List<int>();
            foreach (int v in d.Values)
            {
                list.Add(v);
            }
            Assert.That(list, Is.EquivalentTo(values));
        }

        /// <summary>
        /// A class whose instances can be used as keys to test a hash table. Hash codes are set
        /// via a parameter to the constructor. Instances may be constructed either to be equal to all
        /// other instances, or to be equal only to instances with the same hash code or those instances
        /// that are equal to all instances. This is an incorrect implementation of the GetHashCode method 
        /// and equality (which isn't even transitive), but it is useful in testing a hash table; for
        /// example, we can use an instance that is equal to all other instances when doing a lookup to
        /// see whether the linked list to which this instance hashes is empty.
        /// </summary>
        private class HashTableTester
        {
            /// <summary>
            /// The hash code.
            /// </summary>
            private int _hashCode;

            /// <summary>
            /// Indicates whether this instance is equal to all other instances.
            /// </summary>
            private bool _equalsAll;

            /// <summary>
            /// Constructs a new instance having the given hash code.
            /// </summary>
            /// <param name="hashCode">The hash code.</param>
            /// <param name="equalsAll">Indicates whether this instance is equal to all other
            /// instances.</param>
            public HashTableTester(int hashCode, bool equalsAll)
            {
                _hashCode = hashCode;
                _equalsAll = equalsAll;
            }

            /// <summary>
            /// Constructs a new instance having the given hash code and which
            /// is not equal to all other instances.
            /// </summary>
            /// <param name="hashCode">The hash code.</param>
            public HashTableTester(int hashCode) : this(hashCode, false)
            {

            }

            /// <summary>
            /// Gets the hash code.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                return _hashCode;
            }

            /// <summary>
            /// Determines whether the given object is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare to.</param>
            /// <returns>Whether obj is equal to this instance.</returns>
            public override bool Equals(object obj)
            {
                if (obj is HashTableTester)
                {
                    HashTableTester x = (HashTableTester)obj;
                    return x == this;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Determines whether the given instances are equal.
            /// </summary>
            /// <param name="x">One instance to compare.</param>
            /// <param name="y">The other instance.</param>
            /// <returns>Whether x and y are equal.</returns>
            public static bool operator ==(HashTableTester x, HashTableTester y)
            {
                if (Equals(x, null))
                {
                    return Equals(y, null);
                }
                else if (Equals(y, null))
                {
                    return false;
                }
                else
                {
                    return x._hashCode == y._hashCode || x._equalsAll || y._equalsAll;
                }
            }

            /// <summary>
            /// Determines whether the given instances are different.
            /// </summary>
            /// <param name="x">One instance to compare.</param>
            /// <param name="y">The other instance.</param>
            /// <returns>Whether x and y are different.</returns>
            public static bool operator !=(HashTableTester x, HashTableTester y)
            {
                return !(x == y);
            }
        }

    }
}