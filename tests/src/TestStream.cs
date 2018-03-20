//======================================================================================================================
namespace hhlogic.streams.test {
//----------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using hhlogic.streams;

using Tuple = hhlogic.streams.Tuple;

#if __MonoCS__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using CollectionAssert = NUnit.Framework.Assert;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
//======================================================================================================================


//======================================================================================================================
    struct Vector2
{
  //--------------------------------------------------------------------------------------------------------------------
  public static Vector2 zero = new Vector2(0.0f, 0.0f);
  //--------------------------------------------------------------------------------------------------------------------
  public float x;
  public float y;
  //--------------------------------------------------------------------------------------------------------------------
  public Vector2(float x, float y)
  {
    this.x = x;
    this.y = y;
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
struct Vector3
{
  //--------------------------------------------------------------------------------------------------------------------
  public static Vector3 zero = new Vector3(0.0f, 0.0f, 0.0f);
  //--------------------------------------------------------------------------------------------------------------------
  public float x;
  public float y;
  public float z;
  //--------------------------------------------------------------------------------------------------------------------
  public Vector3(float x, float y, float z)
  {
    this.x = x;
    this.y = y;
    this.z = z;
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
[TestClass]
public class TestStream
{
  //--------------------------------------------------------------------------------------------------------------------
  public class TestEnumerable : IEnumerable
  {
    public IEnumerator GetEnumerator()
    {
      return new TestEnumerator();
    }

    private class TestEnumerator : IEnumerator
    {
      private string[] values = new string[]{"a", "b", "c", "d", "e"};
      private int index = -1;

      public void Reset()
      {
        index = -1;
      }

      public bool MoveNext()
      {
        return ++index < values.Length;
      }

      public object Current {get {return values[index];}}
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  private Func<Func<Maybe<char>>> supplier = () =>
  {
    char c = 'a';
    return () => c < 'f'? Maybe.of(c++) : Maybe<char>.nothing;
  };
  //--------------------------------------------------------------------------------------------------------------------
  private Predicate<Predicate<string>> iterationSupplier = (Predicate<string> f) =>
  {
    var values = new string[]{"a", "b", "c", "d", "e"};

    foreach(var c in values)
      if(!f(c))
        return false;

    return true;
  };
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testEmpty()
  {
    {
      int count = 0;
      Stream.empty<string>().forEach(i => count++);
      Assert.AreEqual(0, count);
    }
    {
      int count = 0;
      Stream.empty<string>().filter(i => {count++; return true;}).forEach(i => {});
      Assert.AreEqual(0, count);
    }
    {
      int count = 0;
      Stream.empty<string>().map(i => {count++; return 1;}).forEach(i => {});
      Assert.AreEqual(0, count);
    }
    {
      int count = 0;
      Stream.empty<string>().first().ifPresent(() => count++);
      Assert.AreEqual(0, count);
    }
    {
      bool executed = false;
      Stream.empty<string>().ifPresent(i => executed = true).toArray();
      Assert.IsFalse(executed);
    }
    {
      int count = 0;
      Stream.empty<string>().map(2u, i => {count++; return 1;}).forEach(i => {});
      Assert.AreEqual(0, count);
    }
    {
      int count = 0;
      Stream.empty<string>().mapIfPresent(i => {count++; return Maybe.of(1);}).forEach(i => {});
      Assert.AreEqual(0, count);
    }
    {
      var s = Stream.empty<string>().join(Stream.of("a", "b", "c"));
      Assert.AreEqual(3u, s.count());
      CollectionAssert.AreEqual(new string[]{"a", "b", "c"}, s.toArray());
    }
    {
      var s = Stream.empty<string>().join(new string[]{"a", "b", "c"});
      Assert.AreEqual(3u, s.count());
      CollectionAssert.AreEqual(new string[]{"a", "b", "c"}, s.toArray());
    }
    {
      var s = Stream.empty<string>().append("c");
      Assert.AreEqual(1u, s.count());
      CollectionAssert.AreEqual(new string[]{"c"}, s.toArray());
    }
    {
      var s = Stream.empty<string>().prepend("c");
      Assert.AreEqual(1u, s.count());
      CollectionAssert.AreEqual(new string[]{"c"}, s.toArray());
    }

    Assert.IsFalse(Stream.empty<string>().reduce((a, b) => a + b).isPresent());
    Assert.AreEqual("a", Stream.empty<string>().reduce("a", (a, b) => a + b));
    Assert.AreEqual(0, Stream.empty<string>().reduce(0, (a, b) => a + 1));
    Assert.AreEqual("a", Stream.empty<string>().reduce(Tuple.of<string, Func<string, string, string>>("a", (a, b) => a + b)));
    Assert.AreEqual(0, Stream.empty<string>().reduce(Tuple.of<int, Func<int, string, int>>(0, (a, b) => a + 1)));

    Assert.IsTrue(Stream.empty<string>().isEmpty());
    Assert.IsFalse(Stream.empty<string>().isNotEmpty());
    Assert.IsTrue(Stream.empty<string>().join(Stream.empty<string>()).isEmpty());
    Assert.IsTrue(Stream.empty<string>().tail().isEmpty());
    Assert.IsTrue(Stream.empty<string>().limit(4).isEmpty());
    Assert.IsTrue(Stream.empty<string>().skip(2).isEmpty());
    Assert.IsTrue(Stream.empty<string>().repeat(5).isEmpty());
    Assert.IsTrue(Stream.empty<string>().repeat(i => 4, i => 7, i => 3).isEmpty());
    Assert.IsTrue(Stream.empty<string>().repeat(Stream.of(new Func<string, int>[]{i => 4, i => 7, i => 3})).isEmpty());
    Assert.IsTrue(Stream.empty<string>().snapshot().isEmpty());
    Assert.IsTrue(Stream.empty<string>().reverse().isEmpty());
    Assert.IsTrue(Stream.empty<int>().sort((a, b) => a < b? -1 : 1).isEmpty());
    Assert.IsTrue(Stream.empty<string>().shuffle().isEmpty());
    Assert.IsTrue(Stream.empty<string>().shuffle(1234).isEmpty());
    Assert.IsTrue(Stream.empty<string>().distinct().isEmpty());
    Assert.IsFalse(Stream.empty<string>().first().isPresent());
    Assert.IsFalse(Stream.empty<string>().head().isPresent());
    Assert.IsFalse(Stream.empty<string>().last().isPresent());
    Assert.AreEqual(0, Stream.empty<string>().toList().Count);
    Assert.AreEqual(0, Stream.empty<string>().toDictionary(i => Entry.build(i, 100)).Count);
    Assert.AreEqual(0u, Stream.empty<string>().fastCount().get(1000u));
    Assert.AreEqual(0u, Stream.empty<string>().count());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testSingle()
  {
    Assert.AreEqual(1u, Stream.of("value").count());
    Assert.AreEqual("value", Stream.of("value").head().get(""));
    Assert.AreEqual("value", Stream.of("value").last().get(""));
    Assert.AreEqual("value", Stream.of("value").snapshot().first().get(""));
    Assert.IsTrue(Stream.of("value").head().isPresent());
    Assert.IsFalse(Stream.of("value").tail().head().isPresent());
    Assert.IsFalse(Stream.of("value").isEmpty());
    Assert.IsTrue(Stream.of("value").isNotEmpty());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testInt()
  {
    var list = new List<int>();

    Stream.of(new int[]{0, 1, 2, 3}).forEach(i => list.Add(i));
    Assert.AreEqual(4, list.Count);
    Assert.AreEqual(0, list[0]);
    Assert.AreEqual(1, list[1]);
    Assert.AreEqual(2, list[2]);
    Assert.AreEqual(3, list[3]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIntRange()
  {
    var list = new List<int>();
    var strm = Stream.ofRange(0, 4);

    strm.forEach(i => list.Add(i));
    Assert.AreEqual(4, list.Count);
    Assert.AreEqual(0, list[0]);
    Assert.AreEqual(1, list[1]);
    Assert.AreEqual(2, list[2]);
    Assert.AreEqual(3, list[3]);

    Assert.IsFalse(strm.isEmpty());
    Assert.IsTrue(strm.isNotEmpty());

    Assert.AreEqual(0, Stream.ofRange(7, 6).toArray().Length);

    Assert.AreEqual(0, strm.head().get(1000));
    Assert.AreEqual(1, strm.tail().head().get(1000));
    Assert.AreEqual(2, strm.tail().tail().head().get(1000));
    Assert.AreEqual(3, strm.tail().tail().tail().head().get(1000));
    Assert.AreEqual(1000, strm.tail().tail().tail().tail().head().get(1000));
    Assert.AreEqual(1000, strm.tail().tail().tail().tail().tail().head().get(1000));

    int sum = 0;

    strm.forEachWhile(i =>
    {
      sum += i;
      return i < 2;
    });

    Assert.AreEqual(3, sum);
    Assert.AreEqual(4, strm.toList().Count);
    CollectionAssert.AreEqual(new int[]{0, 1, 2, 3}, strm.toList().ToArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testUint()
  {
    var list = new List<uint>();

    Stream.of(new uint[]{0, 1, 2, 3}).forEach(i => list.Add(i));
    Assert.AreEqual(4, list.Count);
    Assert.AreEqual(0u, list[0]);
    Assert.AreEqual(1u, list[1]);
    Assert.AreEqual(2u, list[2]);
    Assert.AreEqual(3u, list[3]);

    Assert.AreEqual(0, Stream.ofRange(7u, 6u).toArray().Length);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testUintRange()
  {
    var list = new List<uint>();
    var strm = Stream.ofRange(0u, 4u);

    strm.forEach(i => list.Add(i));
    Assert.AreEqual(4, list.Count);
    Assert.AreEqual(0u, list[0]);
    Assert.AreEqual(1u, list[1]);
    Assert.AreEqual(2u, list[2]);
    Assert.AreEqual(3u, list[3]);

    Assert.IsFalse(strm.isEmpty());
    Assert.IsTrue(strm.isNotEmpty());

    Assert.AreEqual(0, Stream.ofRange(7u, 6u).toArray().Length);

    Assert.AreEqual(0u, strm.head().get(1000u));
    Assert.AreEqual(1u, strm.tail().head().get(1000u));
    Assert.AreEqual(2u, strm.tail().tail().head().get(1000u));
    Assert.AreEqual(3u, strm.tail().tail().tail().head().get(1000u));
    Assert.AreEqual(1000u, strm.tail().tail().tail().tail().head().get(1000u));
    Assert.AreEqual(1000u, strm.tail().tail().tail().tail().tail().head().get(1000u));

    uint sum = 0u;

    strm.forEachWhile(i =>
    {
      sum += i;
      return i < 2u;
    });

    Assert.AreEqual(3u, sum);
    Assert.AreEqual(4, strm.toList().Count);
    CollectionAssert.AreEqual(new uint[]{0u, 1u, 2u, 3u}, strm.toList().ToArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testForEach()
  {
    List<string> list = new List<string>();

    Stream.of(new string[]{"a", "b", "c"})
      .forEach(i => list.Add(i));

    Assert.AreEqual(3, list.Count);
    Assert.AreEqual("a", list[0]);
    Assert.AreEqual("b", list[1]);
    Assert.AreEqual("c", list[2]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testEnumerable()
  {
    List<string> list = new List<string>();

    Stream.of<string>(new TestEnumerable())
      .forEach(i => list.Add(i));

    Assert.AreEqual(5, list.Count);
    Assert.AreEqual("a", list[0]);
    Assert.AreEqual("b", list[1]);
    Assert.AreEqual("c", list[2]);
    Assert.AreEqual("d", list[3]);
    Assert.AreEqual("e", list[4]);

    Assert.IsFalse(Stream.of<string>(new TestEnumerable()).isEmpty());
    Assert.IsTrue(Stream.of<string>(new TestEnumerable()).isNotEmpty());

    Assert.IsTrue(Stream.of<string>((TestEnumerable) null).isEmpty());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testEnum()
  {
    List<DayOfWeek> list = new List<DayOfWeek>();

    Stream.ofEnum<DayOfWeek>()
      .forEach(i => list.Add(i));

    Assert.AreEqual(7, list.Count);
    Assert.AreEqual(DayOfWeek.sunday, list[0]);
    Assert.AreEqual(DayOfWeek.monday, list[1]);
    Assert.AreEqual(DayOfWeek.tuesday, list[2]);
    Assert.AreEqual(DayOfWeek.wednesday, list[3]);
    Assert.AreEqual(DayOfWeek.thursday, list[4]);
    Assert.AreEqual(DayOfWeek.friday, list[5]);
    Assert.AreEqual(DayOfWeek.saturday, list[6]);

    Assert.IsFalse(Stream.ofEnum<DayOfWeek>().isEmpty());
    Assert.IsTrue(Stream.ofEnum<DayOfWeek>().isNotEmpty());

    Assert.IsTrue(Stream.ofEnum<string>().isEmpty());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testSupplier()
  {
    CollectionAssert.AreEqual(new char[]{'a', 'b', 'c', 'd', 'e'}, Stream.of(supplier()).toArray());
    Assert.AreEqual('a', Stream.of(supplier()).head().get('_'));

    Assert.IsFalse(Stream.of(supplier()).isEmpty());
    Assert.IsTrue(Stream.of(supplier()).isNotEmpty());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIterationSupplier()
  {
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d", "e"}, Stream.of(iterationSupplier).toArray());
    Assert.AreEqual("a", Stream.of(iterationSupplier).head().get("none"));
    Assert.AreEqual("e", Stream.of(iterationSupplier).last().get("none"));

    Assert.AreEqual("b", Stream.of(iterationSupplier).tail().head().get("none"));
    Assert.AreEqual("c", Stream.of(iterationSupplier).tail().tail().head().get("none"));
    Assert.AreEqual("d", Stream.of(iterationSupplier).tail().tail().tail().head().get("none"));
    Assert.AreEqual("e", Stream.of(iterationSupplier).tail().tail().tail().tail().head().get("none"));
    Assert.AreEqual("none", Stream.of(iterationSupplier).tail().tail().tail().tail().tail().head().get("none"));
    Assert.AreEqual("none", Stream.of(iterationSupplier).tail().tail().tail().tail().tail().tail().head().get("none"));

    Assert.IsFalse(Stream.of(iterationSupplier).isEmpty());
    Assert.IsTrue(Stream.of(iterationSupplier).isNotEmpty());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testCount()
  {
    Assert.AreEqual(7u, Stream.ofEnum<DayOfWeek>().count());
    Assert.AreEqual(4u, Stream.of(new int[]{1, 2, 3, 4}).count());
    Assert.AreEqual(5u, Stream.of(new uint[]{1, 2, 3, 4, 5}).count());
    Assert.AreEqual(5u, Stream.of(new string[]{"a", "b", "c", "d", "e"}).count());
    Assert.AreEqual(100u, Stream.ofRange(0, 100).count());
    Assert.AreEqual(100u, Stream.ofRange(0u, 100u).count());
    Assert.AreEqual(5u, Stream.of(supplier()).count());
    Assert.AreEqual(5u, Stream.of(iterationSupplier).count());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFastCount()
  {
    Assert.AreEqual(7u, Stream.ofEnum<DayOfWeek>().fastCount().get(1000u));
    Assert.AreEqual(4u, Stream.of(new int[]{1, 2, 3, 4}).fastCount().get(1000u));
    Assert.AreEqual(5u, Stream.of(new uint[]{1, 2, 3, 4, 5}).fastCount().get(1000u));
    Assert.AreEqual(5u, Stream.of(new string[]{"a", "b", "c", "d", "e"}).fastCount().get(1000u));
    Assert.AreEqual(100u, Stream.ofRange(0, 100).fastCount().get(1000u));
    Assert.AreEqual(100u, Stream.ofRange(0u, 100u).fastCount().get(1000u));

    Assert.IsFalse(Stream.of(new string[]{"a", "b", "c", "d", "e"}).filter(c => !c.Equals("c")).fastCount().isPresent());
    Assert.IsFalse(Stream.of(supplier()).fastCount().isPresent());
    Assert.IsFalse(Stream.of(iterationSupplier).fastCount().isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFilter()
  {
    var strm = Stream.of(new string[]{"a", "b", "c", "d", "e"}).filter(s => !s.Equals("c"));
    Assert.AreEqual(4u, strm.count());

    List<string> list = new List<string>();

    Stream.of(new string[]{"a", "b", "c", "d", "e"})
      .filter(s => !s.Equals("c"))
      .forEach(s => list.Add(s));

    Assert.AreEqual(4, list.Count);
    Assert.AreEqual("a", list[0]);
    Assert.AreEqual("b", list[1]);
    Assert.AreEqual("d", list[2]);
    Assert.AreEqual("e", list[3]);

    Assert.AreEqual('d', Stream.of(supplier()).filter(c => c == 'd').first().get('x'));

    Assert.AreEqual("a", strm.head().get(""));
    Assert.AreEqual("b", strm.tail().head().get(""));
    Assert.AreEqual("d", strm.tail().tail().head().get(""));
    Assert.AreEqual("e", strm.tail().tail().tail().head().get(""));
    Assert.AreEqual("none",  strm.tail().tail().tail().tail().head().get("none"));
    Assert.AreEqual("none",  strm.tail().tail().tail().tail().tail().head().get("none"));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFilterEnumerable()
  {
    Assert.AreEqual(4u, Stream.of<string>(new TestEnumerable()).filter(s => !s.Equals("c")).count());

    List<string> list = new List<string>();

    Stream.of<string>(new TestEnumerable())
      .filter(s => !s.Equals("c"))
      .forEach(s => list.Add(s));

    Assert.AreEqual(4, list.Count);
    Assert.AreEqual("a", list[0]);
    Assert.AreEqual("b", list[1]);
    Assert.AreEqual("d", list[2]);
    Assert.AreEqual("e", list[3]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFilterInt()
  {
    Assert.AreEqual(4u, Stream.of(new int[]{1, 2, 3, 4, 5}).filter(i => i != 4).count());

    List<int> list = new List<int>();

    Stream.of(new int[]{1,2, 3, 4, 5})
      .filter(i => i != 4)
      .forEach(i => list.Add(i));

    Assert.AreEqual(4, list.Count);
    Assert.AreEqual(1, list[0]);
    Assert.AreEqual(2, list[1]);
    Assert.AreEqual(3, list[2]);
    Assert.AreEqual(5, list[3]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFilterUint()
  {
    Assert.AreEqual(4u, Stream.of(new uint[]{1, 2, 3, 4, 5}).filter(i => i != 4).count());

    List<uint> list = new List<uint>();

    Stream.of(new uint[]{1,2, 3, 4, 5})
      .filter(i => i != 4u)
      .forEach(i => list.Add(i));

    Assert.AreEqual(4, list.Count);
    Assert.AreEqual(1u, list[0]);
    Assert.AreEqual(2u, list[1]);
    Assert.AreEqual(3u, list[2]);
    Assert.AreEqual(5u, list[3]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFilterEnum()
  {
    Assert.AreEqual(6u, Stream.ofEnum<DayOfWeek>().filter(d => d != DayOfWeek.monday).count());

    List<DayOfWeek> list = new List<DayOfWeek>();

    Stream.ofEnum<DayOfWeek>()
      .filter(d => d != DayOfWeek.monday)
      .forEach(i => list.Add(i));

    Assert.AreEqual(6, list.Count);
    Assert.AreEqual(DayOfWeek.sunday, list[0]);
    Assert.AreEqual(DayOfWeek.tuesday, list[1]);
    Assert.AreEqual(DayOfWeek.wednesday, list[2]);
    Assert.AreEqual(DayOfWeek.thursday, list[3]);
    Assert.AreEqual(DayOfWeek.friday, list[4]);
    Assert.AreEqual(DayOfWeek.saturday, list[5]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToArray()
  {
    CollectionAssert.AreEqual(
      new string[]{"a", "b", "c", "d", "e"},
      Stream.of(new string[]{"a", "b", "c", "d", "e"}).toArray());

    CollectionAssert.AreEqual(
      new string[]{"a", "b", "d", "e"},
      Stream.of(new string[]{"a", "b", "c", "d", "e"}).filter(s => !s.Equals("c")).toArray());

    {
      Stream<string> stream = Stream.of(new string[]{"a", "b", "d", "e"});
      stream.first();
      CollectionAssert.AreEqual(new string[]{"a", "b", "d", "e"}, stream.toArray());
    }
    {
      Stream<string> stream = Stream.of(new string[]{"a", "b", "d", "e"});
      stream.forEach(i => {});
      CollectionAssert.AreEqual(new string[]{"a", "b", "d", "e"}, stream.toArray());
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToList()
  {
    CollectionAssert.AreEqual(
      new string[]{"a", "b", "c", "d", "e"},
      Stream.of(new string[]{"a", "b", "c", "d", "e"}).toList().ToArray());

    CollectionAssert.AreEqual(
      new string[]{"a", "b", "d", "e"},
      Stream.of(new string[]{"a", "b", "c", "d", "e"}).filter(s => !s.Equals("c")).toList().ToArray());

    {
      Stream<string> stream = Stream.of(new string[]{"a", "b", "d", "e"});
      stream.first();
      CollectionAssert.AreEqual(new string[]{"a", "b", "d", "e"}, stream.toList().ToArray());
    }
    {
      Stream<string> stream = Stream.of(new string[]{"a", "b", "d", "e"});
      stream.forEach(i => {});
      CollectionAssert.AreEqual(new string[]{"a", "b", "d", "e"}, stream.toList().ToArray());
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToDictionary()
  {
    var d = Stream.ofRange(0, 5).toDictionary(i => Entry.build(i, 'a' + i));
    Assert.AreEqual('a', Maybe.of(d, 0).get('_'));
    Assert.AreEqual('b', Maybe.of(d, 1).get('_'));
    Assert.AreEqual('c', Maybe.of(d, 2).get('_'));
    Assert.AreEqual('d', Maybe.of(d, 3).get('_'));
    Assert.AreEqual('e', Maybe.of(d, 4).get('_'));
    Assert.AreEqual('_', Maybe.of(d, 5).get('_'));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToArrayEnumerable()
  {
    CollectionAssert.AreEqual(
      new string[]{"a", "b", "c", "d", "e"},
      Stream.of<string>(new TestEnumerable()).toArray());

    CollectionAssert.AreEqual(
      new string[]{"a", "b", "d", "e"},
      Stream.of<string>(new TestEnumerable()).filter(s => !s.Equals("c")).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToArrayInt()
  {
    CollectionAssert.AreEqual(
      new int[]{1, 2, 3, 4, 5},
      Stream.of(new int[]{1, 2, 3, 4, 5}).toArray());

    CollectionAssert.AreEqual(
      new int[]{1, 2, 3, 5},
      Stream.of(new int[]{1, 2, 3, 4, 5}).filter(i => i != 4).toArray());

    CollectionAssert.AreEqual(
      new int[]{1, 2, 3, 4, 5},
      Stream.ofRange(1, 6).toArray());

    CollectionAssert.AreEqual(
      new int[]{1, 2, 3, 5},
      Stream.ofRange(1, 6).filter(i => i != 4).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToArrayUint()
  {
    CollectionAssert.AreEqual(
      new uint[]{1, 2, 3, 4, 5},
      Stream.of(new uint[]{1, 2, 3, 4, 5}).toArray());

    CollectionAssert.AreEqual(
      new uint[]{1, 2, 3, 5},
      Stream.of(new uint[]{1, 2, 3, 4, 5}).filter(i => i != 4u).toArray());

    CollectionAssert.AreEqual(
      new uint[]{1, 2, 3, 4, 5},
      Stream.ofRange(1u, 6u).toArray());

    CollectionAssert.AreEqual(
      new uint[]{1, 2, 3, 5},
      Stream.ofRange(1u, 6u).filter(i => i != 4).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testToArrayEnum()
  {
    var allDays = new DayOfWeek[]{DayOfWeek.sunday,
                                  DayOfWeek.monday,
                                  DayOfWeek.tuesday,
                                  DayOfWeek.wednesday,
                                  DayOfWeek.thursday,
                                  DayOfWeek.friday,
                                  DayOfWeek.saturday};

    var allButThursday = new DayOfWeek[]{DayOfWeek.sunday,
                                         DayOfWeek.monday,
                                         DayOfWeek.tuesday,
                                         DayOfWeek.wednesday,
                                         DayOfWeek.friday,
                                         DayOfWeek.saturday};

    CollectionAssert.AreEqual(allDays, Stream.of(allDays).toArray());
    CollectionAssert.AreEqual(allDays, Stream.ofEnum<DayOfWeek>().toArray());
    CollectionAssert.AreEqual(allButThursday, Stream.of(allDays).filter(d => d != DayOfWeek.thursday).toArray());

    CollectionAssert.AreEqual(allButThursday,
                              Stream.ofEnum<DayOfWeek>().filter(d => d != DayOfWeek.thursday).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFirst()
  {
    Assert.AreEqual("a", Stream.of(new string[]{"a", "b", "c", "d", "e"}).first().get(() => "d"));
    Assert.AreEqual('a', Stream.of(supplier()).first().get('x'));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testHead()
  {
    Assert.AreEqual("a", Stream.of(new string[]{"a", "b", "c", "d", "e"}).head().get(() => "d"));
    Assert.AreEqual("a", Stream.of(new string[]{"a"}).head().get(() => "d"));
    Assert.AreEqual("b", Stream.of(new string[]{"a", "b"}).filter(s => !s.Equals("a")).head().get(() => "d"));

    Assert.AreEqual("b", Stream.of(new string[]{"a", "b", "c", "d", "e"})
                          .filter(s => !s.Equals("d")).tail().head().get(() => "d"));

    Assert.AreEqual('a', Stream.of(supplier()).head().get('x'));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testTail()
  {
    var allButSunday = new DayOfWeek[]{DayOfWeek.monday,
                                       DayOfWeek.tuesday,
                                       DayOfWeek.wednesday,
                                       DayOfWeek.thursday,
                                       DayOfWeek.friday,
                                       DayOfWeek.saturday};

    CollectionAssert.AreEqual(new string[]{"b", "c", "d", "e"},
                    Stream.of(new string[]{"a", "b", "c", "d", "e"}).tail().toArray());

    CollectionAssert.AreEqual(new int[]{2, 3, 4, 5}, Stream.ofRange(1, 6).tail().toArray());
    CollectionAssert.AreEqual(new uint[]{2, 3, 4, 5}, Stream.ofRange(1u, 6u).tail().toArray());

    CollectionAssert.AreEqual(allButSunday, Stream.ofEnum<DayOfWeek>().tail().toArray());

    CollectionAssert.AreEqual(new string[]{"b", "c", "e"},
                              Stream.of(new string[]{"a", "b", "c", "d", "e"})
                                .filter(s => !s.Equals("d")).tail().toArray());

    CollectionAssert.AreEqual(new string[]{"2", "3", "4", "5"},
                              Stream.ofRange(1, 6).map(i => i.ToString()).tail().toArray());

    CollectionAssert.AreEqual(new int[]{2, 3, 4, 5, 6},
                              Stream.of(new int[][]{new int[]{1, 2}, new int[]{3, 4}, new int[]{5, 6}})
                                .flatMap(a => Stream.of(a)).tail().toArray());

    Assert.IsTrue(Stream.of(new string[]{"a"}).tail().head().isNotPresent());

    CollectionAssert.AreEqual(new string[]{"e"},
                              Stream.of(new string[]{"a", "b", "c", "d", "e"}).tail().tail().tail().tail().toArray());

    CollectionAssert.AreEqual(new string[]{"e"},
                              Stream.of(new string[]{"a", "b", "c", "d", "e"})
                                .filter(s => !s.Equals("c")).tail().tail().tail().toArray());

    CollectionAssert.AreEqual(new string[]{"5"},
                              Stream.ofRange(1, 6).map(i => i.ToString()).tail().tail().tail().tail().toArray());

    CollectionAssert.AreEqual(new int[]{6},
                              Stream.of(new int[][]{new int[]{1, 2}, new int[]{3, 4}, new int[]{5, 6}})
                                .flatMap(a => Stream.of(a)).tail().tail().tail().tail().tail().toArray());

    Assert.AreEqual(0,
                    Stream.of(new string[]{"a", "b"}).tail().tail().tail().tail().tail().tail().tail()
                      .toArray().Length);

    CollectionAssert.AreEqual(new char[]{'b', 'c', 'd', 'e'}, Stream.of(supplier()).tail().toArray());
    CollectionAssert.AreEqual(new char[]{'c', 'd', 'e'}, Stream.of(supplier()).tail().tail().toArray());
    CollectionAssert.AreEqual(new char[]{'d', 'e'}, Stream.of(supplier()).tail().tail().tail().toArray());
    CollectionAssert.AreEqual(new char[]{'e'}, Stream.of(supplier()).tail().tail().tail().tail().toArray());
    CollectionAssert.AreEqual(new char[]{}, Stream.of(supplier()).tail().tail().tail().tail().tail().toArray());
    CollectionAssert.AreEqual(new char[]{}, Stream.of(supplier()).tail().tail().tail().tail().tail().tail().toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testMap()
  {
    CollectionAssert.AreEqual(new string[]{"0", "1", "2", "3", "4", "5",}, Stream.ofRange(0, 6).map(i => i.ToString()).toArray());
    CollectionAssert.AreEqual(new int[]{102, 103, 104, 105, 106}, Stream.of(supplier()).map(c => c + 5).toArray());
    Assert.AreEqual("0", Stream.ofRange(0, 6).map(i => i.ToString()).head().get("none"));
    Assert.AreEqual("5", Stream.ofRange(0, 6).map(i => i.ToString()).last().get("none"));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFlatMap()
  {
    string[][] values = new string[][]{new string[]{"a", "b"}, new string[]{"c", "d"}, new string[]{"e", "f"}};
    var strm = Stream.of(values).flatMap(a => Stream.of(a));

    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d", "e", "f"}, strm.toArray());
    Assert.AreEqual("a", strm.first().get("none"));
    Assert.AreEqual("f", strm.last().get("none"));
    Assert.AreEqual(6u, strm.count());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testImmutable()
  {
    Stream<string> stream = Stream.of(new string[]{"a", "b", "c", "d", "e"}); 
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d", "e"}, stream.toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d", "e"}, stream.toArray());

    Assert.AreEqual("a", stream.first().get(() => "d"));
    Assert.AreEqual("a", stream.first().get(() => "d"));

    Assert.AreEqual("b", stream.tail().first().get(() => "d"));
    Assert.AreEqual("b", stream.tail().first().get(() => "d"));

    Stream<char> cStream = Stream.of(supplier());
    CollectionAssert.AreEqual(new char[]{'a', 'b', 'c', 'd', 'e'}, cStream.toArray());
    CollectionAssert.AreEqual(new char[]{'a', 'b', 'c', 'd', 'e'}, cStream.toArray());

    Maybe<int>[] maybeArray = Stream.ofRange(0, 4).map(i => Maybe.of(i)).toArray();
    Stream<int> intStream = Stream.ofMaybe(maybeArray);
    CollectionAssert.AreEqual(new int[]{0, 1, 2, 3}, intStream.toArray());

    maybeArray[2] = Maybe<int>.nothing;
    CollectionAssert.AreEqual(new int[]{0, 1, 3}, intStream.toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testSnapshot()
  {
    CollectionAssert.AreEqual(new int[]{1, 2, 3, 4}, Stream.ofRange(1, 5).snapshot().toArray());
    CollectionAssert.AreEqual(new uint[]{1u, 2u, 3u, 4u}, Stream.ofRange(1u, 5u).snapshot().toArray());
    CollectionAssert.AreEqual(new int[]{1, 2, 3, 4}, Stream.of(1, 2, 3, 4).snapshot().toArray());

    {
      int count = 0;
      var s = Stream.of(0, 1, 2, 3, 4).map(i => 'a' + i).ifPresent(i => count++);
      s.toArray();
      s.toArray();
      Assert.AreEqual(10, count);
    }
    {
      int count = 0;
      var s = Stream.of(0, 1, 2, 3, 4).map(i => 'a' + i).ifPresent(i => count++).snapshot();
      s.toArray();
      s.toArray();
      Assert.AreEqual(5, count);
    }

  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testLast()
  {
    var abcde = Stream.of("a", "b", "c", "d", "e");

    Assert.AreEqual("e", abcde.last().get(() => "x"));
    Assert.AreEqual("e", abcde.tail().last().get(() => "x"));
    Assert.AreEqual("e", abcde.tail().tail().tail().tail().last().get(() => "x"));
    Assert.AreEqual("x", abcde.tail().tail().tail().tail().tail().last().get(() => "x"));
    Assert.AreEqual("x", abcde.tail().tail().tail().tail().tail().tail().last().get(() => "x"));

    Assert.AreEqual(5, Stream.ofRange(1, 6).last().get(() => 100));
    Assert.AreEqual(5u, Stream.ofRange(1u, 6u).last().get(() => 100));

    Assert.AreEqual("e", abcde.filter(s => !s.Equals("c")).last().get(() => "x"));
    Assert.AreEqual("5", Stream.ofRange(1, 6).map(i => i.ToString()).last().get(() => "x"));

    Assert.AreEqual(6, Stream.of(new int[]{1, 2}, new int[]{3, 4}, new int[]{5, 6})
                        .flatMap(i => Stream.of(i)).last().get(() => 100));

    Assert.AreEqual("e", Stream.of<string>(new TestEnumerable()).last().get(() => "x"));
    Assert.AreEqual(DayOfWeek.saturday, Stream.ofEnum<DayOfWeek>().last().get(() => DayOfWeek.monday));

    Assert.AreEqual('e', Stream.of(supplier()).last().get('x'));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testReduce()
  {
    Assert.AreEqual(10, Stream.ofRange(0, 5).reduce((a, b) => a + b).get(0));
    Assert.AreEqual(17, Stream.ofRange(0, 5).reduce(7, (a, b) => a + b));
    Assert.AreEqual(120, Stream.ofRange(1, 6).reduce((a, b) => a * b).get(-1));
    Assert.AreEqual(17, Stream.ofRange(0, 5).reduce(Tuple.of<int, Func<int, int, int>>(7, (a, b) => a + b)));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testAppend()
  {
    var strm = Stream.of("a", "b", "c").append("d");

    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d"}, strm.toArray());
    CollectionAssert.AreEqual(new string[]{"d"}, Stream.empty<string>().append("d").toArray());
    CollectionAssert.AreEqual(new string[]{"b", "c", "d"}, strm.tail().toArray());
    Assert.AreEqual("d", strm.last().get("none"));

    Assert.AreEqual("a", strm.head().get("none"));
    Assert.AreEqual("b", strm.tail().head().get("none"));
    Assert.AreEqual("c", strm.tail().tail().head().get("none"));
    Assert.AreEqual("d", strm.tail().tail().tail().head().get("none"));
    Assert.AreEqual("none", strm.tail().tail().tail().tail().head().get("none"));
    Assert.AreEqual("none", strm.tail().tail().tail().tail().tail().head().get("none"));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testPrepend()
  {
    var strm = Stream.of("a", "b", "c").prepend("d");

    CollectionAssert.AreEqual(new string[]{"d", "a", "b", "c"}, strm.toArray());
    CollectionAssert.AreEqual(new string[]{"d"}, Stream.empty<string>().prepend("d").toArray());
    Assert.AreEqual("c", strm.last().get("none"));

    Assert.AreEqual("d", strm.head().get("none"));
    Assert.AreEqual("a", strm.tail().head().get("none"));
    Assert.AreEqual("b", strm.tail().tail().head().get("none"));
    Assert.AreEqual("c", strm.tail().tail().tail().head().get("none"));
    Assert.AreEqual("none", strm.tail().tail().tail().tail().head().get("none"));
    Assert.AreEqual("none", strm.tail().tail().tail().tail().tail().head().get("none"));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testJoin()
  {
    var expected = new string[]{"a", "b", "c", "d", "e", "f"};

    CollectionAssert.AreEqual(expected, Stream.of("a", "b", "c").join(Stream.of("d", "e", "f")).toArray());
    CollectionAssert.AreEqual(expected, Stream.of("a", "b", "c").join(new string[]{"d", "e", "f"}).toArray());

    CollectionAssert.AreEqual(new string[]{"a", "b", "c"}, Stream.empty<string>().join(Stream.of("a", "b", "c")).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testMapIfPresent()
  {
    Stream<Maybe<string>> charStream
      = Stream.of(Maybe.of("a"), Maybe.of("b"), Maybe.of("c"), Maybe<string>.nothing, Maybe.of("e"));

    Assert.AreEqual(5u, charStream.count());
    Assert.AreEqual(4u, charStream.mapIfPresent(m => m).count());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "e"}, charStream.mapIfPresent(m => m).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testLimit()
  {
    Assert.AreEqual(3u, Stream.of("a", "b", "c", "d", "e").limit(3).count());
    Assert.AreEqual(0u, Stream.of("a", "b", "c", "d", "e").limit(0).count());
    Assert.AreEqual(5u, Stream.of("a", "b", "c", "d", "e").limit(10).count());

    CollectionAssert.AreEqual(new string[]{"a", "b", "c"}, Stream.of("a", "b", "c", "d", "e").limit(3).toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b"}, Stream.of("a", "b").limit(3).toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d", "e"}, Stream.of("a", "b", "c", "d", "e").limit(10).toArray());

    Assert.AreEqual("c", Stream.of("a", "b", "c", "d", "e").limit(3).last().get("x"));
    Assert.AreEqual("x", Stream.of("a", "b", "c", "d", "e").limit(0).last().get("x"));

    Assert.AreEqual("a", Stream.of("a", "b", "c", "d", "e").limit(3).head().get("x"));
    Assert.AreEqual("x", Stream.of("a", "b", "c", "d", "e").limit(0).head().get("x"));

    Assert.AreEqual(2u, Stream.of("a", "b", "c", "d", "e").limit(3).tail().count());
    CollectionAssert.AreEqual(new string[]{"b", "c"}, Stream.of("a", "b", "c", "d", "e").limit(3).tail().toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testSkip()
  {
    Assert.AreEqual(2u, Stream.of("a", "b", "c", "d", "e").skip(3).count());
    Assert.AreEqual(5u, Stream.of("a", "b", "c", "d", "e").skip(0).count());
    Assert.AreEqual(0u, Stream.of("a", "b", "c", "d", "e").skip(10).count());
    Assert.AreEqual("e", Stream.of("a", "b", "c", "d", "e").skip(2).last().get("none"));
    Assert.AreEqual("none", Stream.of("a", "b", "c", "d", "e").skip(20).last().get("none"));

    CollectionAssert.AreEqual(new string[]{"d", "e"}, Stream.of("a", "b", "c", "d", "e").skip(3).toArray());
    CollectionAssert.AreEqual(new string[]{}, Stream.of("a", "b").skip(3).toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d", "e"}, Stream.of("a", "b", "c", "d", "e").skip(0).toArray());
    CollectionAssert.AreEqual(new string[]{"c", "d"}, Stream.of("a", "b", "c", "d", "e").skip(2).limit(2).toArray());
    Assert.AreEqual("e", Stream.of("a", "b", "c", "d", "e").skip(2).skip(2).head().get(""));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testReverse()
  {
    var s = Stream.of("a", "b", "c", "d", "e").reverse();
    CollectionAssert.AreEqual(new string[]{"e", "d", "c", "b", "a"}, s.toArray());
    CollectionAssert.AreEqual(new string[]{"e", "d", "c", "b", "a"}, s.snapshot().toArray());
    Assert.AreEqual("e", s.head().get("none"));
    Assert.AreEqual("a", s.last().get("none"));
    Assert.AreEqual("d", s.tail().head().get("none"));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testRepeat()
  {
    CollectionAssert.AreEqual(new int[]{}, Stream.ofRange(1, 4).repeat(0).toArray());
    CollectionAssert.AreEqual(new int[]{1, 2, 3}, Stream.ofRange(1, 4).repeat(1).toArray());
    CollectionAssert.AreEqual(new int[]{1, 2, 3, 1, 2, 3}, Stream.ofRange(1, 4).repeat(2).toArray());
    CollectionAssert.AreEqual(new int[]{1, 2, 3, 1, 2, 3, 1, 2, 3}, Stream.ofRange(1, 4).repeat(3).toArray());

    CollectionAssert.AreEqual(new string[]{}, Stream.of("a", "b", "c").repeat(0).toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c"}, Stream.of("a", "b", "c").repeat(1).toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "a", "b", "c"}, Stream.of("a", "b", "c").repeat(2).toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "a", "b", "c", "a", "b", "c"},
      Stream.of("a", "b", "c").repeat(3).toArray());

    CollectionAssert.AreEqual(new int[]{0, 1, 2, 6, 7, 8}, Stream.ofRange(1, 4).repeat(i => i - 1, i => i + 5).toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testMergeMap()
  {
    Stream<int> s = Stream.ofRange(1, 11).map(2, (int[] i) => i[0] + i[1]);
    Assert.AreEqual(5u, s.count());
    Assert.AreEqual(4u, Stream.ofRange(1, 10).map(2, (int[] i) => i[0] + i[1]).count());
    Assert.AreEqual(0u, Stream.ofRange(1, 10).map(0, (int[] i) => i[0] + i[1]).count());

    {
      var executedCount = 0;
      s.forEach(i => executedCount++);
      Assert.AreEqual(5, executedCount);
    }
    {
      var executedCount = 0;
      Stream.ofRange(1, 11).map(0, (int[] i) => i[0] + i[1]).forEach(i => executedCount++);
      Assert.AreEqual(0, executedCount);
    }

    Assert.AreEqual(3, s.head().get(0));
    Assert.AreEqual(7, s.tail().head().get(0));
    Assert.AreEqual(11, s.tail().tail().head().get(0));
    Assert.AreEqual(15, s.tail().tail().tail().head().get(0));
    Assert.AreEqual(19, s.tail().tail().tail().tail().head().get(0));
    Assert.AreEqual(0, s.tail().tail().tail().tail().tail().head().get(0));
    Assert.IsFalse(s.tail().tail().tail().tail().tail().head().isPresent());

    Assert.AreEqual(55, s.reduce((a, b) => a + b).get(0));
    Assert.AreEqual(65835, s.reduce((a, b) => a * b).get(0));
    Assert.AreEqual(3628800,  Stream.ofRange(1, 11).reduce((a, b) => a*b).get(0));


    float[] values = {1.6f, 3.4f, 0.0f, -5.6f, -10.9f, -2.3f, 7.6f, -11.3f, 3.5f};
    Stream<float> sf = Stream.of(values);
    Stream<Vector3> sv3 = sf.map(3, (float[] f) => new Vector3(f[0], f[1], f[2]));

    Assert.AreEqual(new Vector3(1.6f, 3.4f, 0.0f), sv3.head().get(Vector3.zero));
    Assert.AreEqual(new Vector3( -5.6f, -10.9f, -2.3f), sv3.tail().head().get(Vector3.zero));
    Assert.AreEqual(new Vector3(7.6f, -11.3f, 3.5f), sv3.tail().tail().head().get(Vector3.zero));
    Assert.AreEqual(Vector3.zero, sv3.tail().tail().tail().head().get(Vector3.zero));

    Stream<Vector2> sv2 = sf.map(2, (float[] f) => new Vector2(f[0], f[1]));
    Assert.AreEqual(new Vector2(1.6f, 3.4f), sv2.head().get(Vector2.zero));
    Assert.AreEqual(new Vector2(0.0f, -5.6f), sv2.tail().head().get(Vector2.zero));
    Assert.AreEqual(new Vector2(-10.9f, -2.3f), sv2.tail().tail().head().get(Vector2.zero));
    Assert.AreEqual(new Vector2(7.6f, -11.3f), sv2.tail().tail().tail().head().get(Vector2.zero));
    Assert.AreEqual(Vector2.zero, sv2.tail().tail().tail().tail().head().get(Vector2.zero));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testSort()
  {
    var s = Stream.of('e', 'b', 'f', 'a').sort((a, b) => a < b? -1 : 1);
    CollectionAssert.AreEqual(new char[]{'a', 'b', 'e', 'f'}, s.toArray());
    CollectionAssert.AreEqual(new char[]{'a', 'b', 'e', 'f'}, s.snapshot().toArray());
    Assert.AreEqual('a', s.head().get('_'));
    Assert.AreEqual('f', s.last().get('_'));
    Assert.AreEqual('b', s.tail().head().get('_'));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testShuffle()
  {
    var expected = new char[]{'a', 'b', 'e', 'f'};

    {
      var s = Stream.of('a', 'b', 'e', 'f').shuffle();
      var arry1 = s.toArray();
      var arry2 = s.toArray();
      CollectionAssert.AreNotEqual(new char[]{'a', 'b', 'e', 'f'}, arry1);
      CollectionAssert.AreNotEqual(new char[]{'a', 'b', 'e', 'f'}, arry2);
      CollectionAssert.AreEqual(arry1, arry2);
      CollectionAssert.AreEqual(arry1, s.snapshot().toArray());
      CollectionAssert.AreEqual(expected, Stream.of(arry1).sort((a, b) => a < b? -1 : 1).toArray());
      CollectionAssert.AreEqual(expected, Stream.of(arry2).sort((a, b) => a < b? -1 : 1).toArray());
      Assert.AreEqual(s.head().get('_'), s.head().get('_'));
      Assert.AreEqual(s.last().get('_'), s.last().get('_'));
      Assert.AreEqual(s.tail().head().get('_'), s.tail().head().get('_'));
    }
    {
      var s = Stream.of('a', 'b', 'e', 'f');
      var arry1 = s.shuffle(937291).toArray();
      var arry2 = s.shuffle(937291).toArray();
      CollectionAssert.AreNotEqual(new char[]{'a', 'b', 'e', 'f'}, arry1);
      CollectionAssert.AreNotEqual(new char[]{'a', 'b', 'e', 'f'}, arry2);
      CollectionAssert.AreEqual(arry1, arry2);
      CollectionAssert.AreEqual(arry1, s.shuffle(937291).snapshot().toArray());
      CollectionAssert.AreEqual(expected, Stream.of(arry1).sort((a, b) => a < b? -1 : 1).toArray());
      CollectionAssert.AreEqual(expected, Stream.of(arry2).sort((a, b) => a < b? -1 : 1).toArray());
      Assert.AreEqual(s.head().get('_'), s.head().get('_'));
      Assert.AreEqual(s.last().get('_'), s.last().get('_'));
      Assert.AreEqual(s.tail().head().get('_'), s.tail().head().get('_'));
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testDistinct()
  {
    CollectionAssert.AreEqual(new int[]{1, 2, 3, 4}, Stream.of(1, 2, 1, 3, 4, 4).distinct().toArray());
    CollectionAssert.AreEqual(new string[]{"a", "b", "c", "d"}, Stream.of("a", "b", "c", "b", "a", "d", "d").distinct().toArray());
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================
} // End Namespace
//======================================================================================================================
