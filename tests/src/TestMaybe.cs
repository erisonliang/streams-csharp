//======================================================================================================================
namespace hhlogic.streams.test {
//----------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hhlogic.streams;

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
public enum DayOfWeek {sunday = 0, monday, tuesday, wednesday, thursday, friday, saturday};
//======================================================================================================================


//======================================================================================================================
class TestDisposable : IDisposable
{
  //--------------------------------------------------------------------------------------------------------------------
  private bool isDisposed = false;
  //--------------------------------------------------------------------------------------------------------------------
  public void Dispose()
  {
    isDisposed = true;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override bool Equals(Object that)
  {
    return isDisposed? that == null : this == that;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
[TestClass]
public class TestMaybe
{
  //--------------------------------------------------------------------------------------------------------------------
  private static Dictionary<string, string> crew = new Dictionary<string, string>()
    {{"Captain", "Kirk"}, {"Lieutenant", "Uhura"}, {"Engineer", "Scotty"}, {"Officer", "Spock"}, {"Doctor", "McCoy"}};
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIsPresent()
  {
    Assert.IsTrue(Maybe.of(3).isPresent());

    {
      Object o = null;
      Assert.IsFalse(Maybe.of(o).isPresent());
    }
    {
      TestDisposable o = new TestDisposable();
      Assert.IsTrue(Maybe.of(o).isPresent());
      o.Dispose();
      Assert.IsFalse(Maybe.of(o).isPresent());
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIsNotPresent()
  {
    Assert.IsFalse(Maybe.of(3).isNotPresent());

    {
      Object o = null;
      Assert.IsTrue(Maybe.of(o).isNotPresent());
    }
    {
      TestDisposable o = new TestDisposable();
      Assert.IsFalse(Maybe.of(o).isNotPresent());
      o.Dispose();
      Assert.IsTrue(Maybe.of(o).isNotPresent());
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIfPresent()
  {
    {
      bool result = false;
      Maybe.of(3).ifPresent(i => result = true);
      Assert.IsTrue(result);
    }
    {
      bool result = false;
      Object o = null;
      Maybe.of(o).ifPresent(i => result = true);
      Assert.IsFalse(result);
    }
    {
      TestDisposable o = new TestDisposable();
      Maybe<TestDisposable> m = Maybe.of(o);

      bool result = false;
      m.ifPresent(i => result = true);
      Assert.IsTrue(result);

      result = false;
      o.Dispose();
      m.ifPresent(i => result = true);
      Assert.IsFalse(result);
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIfPresentAction()
  {
    {
      bool result = false;
      Maybe.of(3).ifPresent(() => result = true);
      Assert.IsTrue(result);
    }
    {
      bool result = false;
      Object o = null;
      Maybe.of(o).ifPresent(() => result = true);
      Assert.IsFalse(result);
    }
    {
      TestDisposable o = new TestDisposable();
      Maybe<TestDisposable> m = Maybe.of(o);

      bool result = false;
      m.ifPresent(() => result = true);
      Assert.IsTrue(result);

      result = false;
      o.Dispose();
      m.ifPresent(() => result = true);
      Assert.IsFalse(result);
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testIfNotPresent()
  {
    {
      bool result = false;
      Object o = null;
      Maybe.of(o).ifNotPresent(() => result = true);
      Assert.IsTrue(result);
    }
    {
      bool result = false;
      TestDisposable o = new TestDisposable();
      Maybe.of(o).ifNotPresent(() => result = true);
      Assert.IsFalse(result);
    }
    {
      TestDisposable o = new TestDisposable();
      Maybe<TestDisposable> m = Maybe.of(o);

      bool result = false;
      m.ifNotPresent(() => result = true);
      Assert.IsFalse(result);

      result = false;
      o.Dispose();
      m.ifNotPresent(() => result = true);
      Assert.IsTrue(result);
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFilter()
  {
    Assert.IsTrue(Maybe.of(3).filter(i => i == 3).isPresent());
    Assert.IsFalse(Maybe.of(3).filter(i => i == 4).isPresent());
    Assert.IsTrue(Maybe.of(3).filter(() => true).isPresent());
    Assert.IsFalse(Maybe.of(3).filter(() => false).isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testMap()
  {
    bool result = false;
    Maybe.of(4).map(i => "four").ifPresent(s => result = s.Equals("four"));
    Assert.IsTrue(result);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFlatMap()
  {
    bool result = false;
    Maybe.of(4).flatMap(i => Maybe.of("four")).ifPresent(s => result = s.Equals("four"));
    Assert.IsTrue(result);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testFlatMapStream()
  {
    string[] values = Maybe.of(4).flatMap(i => Stream.of("four", "five", "six")).toArray();
    Assert.AreEqual(3, values.Length);
    Assert.AreEqual("four", values[0]);
    Assert.AreEqual("five", values[1]);
    Assert.AreEqual("six", values[2]);
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testOrElseOf()
  {
    Assert.AreEqual("test", Maybe.of("test").orElseOf("123").get(() => ""));
    Assert.AreEqual("123", Maybe.of("test").filter(() => false).orElseOf("123").get(() => ""));

    Assert.AreEqual("test", Maybe.of("test").orElseOf(() => "123").get(() => ""));
    Assert.AreEqual("123", Maybe.of("test").filter(() => false).orElseOf(() => "123").get(() => ""));

    Assert.AreEqual("test", Maybe.of("test").orElseOfFlatMap(Maybe.of("123")).get(() => ""));
    Assert.AreEqual("123", Maybe.of("test").filter(() => false).orElseOfFlatMap(Maybe.of("123")).get(() => ""));

    Assert.AreEqual("test", Maybe.of("test").orElseOfFlatMap(() => Maybe.of("123")).get(() => ""));
    Assert.AreEqual("123", Maybe.of("test").filter(() => false).orElseOfFlatMap(() => Maybe.of("123")).get(() => ""));
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testEmpty()
  {
    Assert.IsFalse(Maybe<int>.nothing.isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testJoin()
  {
    Assert.IsTrue(Maybe.of("a").join(Maybe.of(1)).map(t => t.first.Equals("a") && t.second.Equals(1)).get(false));
    Assert.IsTrue(Maybe.of("a").join(Maybe<int>.nothing).isNotPresent());
    Assert.IsTrue(Maybe<char>.nothing.join(Maybe.of(1)).isNotPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testMaybeInt()
  {
    Assert.AreEqual(578, Maybe.ofInt("578").get(0));
    Assert.AreEqual(-27, Maybe.ofInt("-27").get(0));
    Assert.IsTrue(Maybe.ofInt("578").isPresent());
    Assert.IsTrue(Maybe.ofInt("-27").isPresent());
    Assert.IsFalse(Maybe.ofInt("i27").isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testMaybeUint()
  {
    Assert.AreEqual(9123u, Maybe.ofUint("9123").get(0u));
    Assert.IsTrue(Maybe.ofUint("9123").isPresent());
    Assert.IsFalse(Maybe.ofUint("-27").isPresent());
    Assert.IsFalse(Maybe.ofUint("i27").isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testEnum()
  {
    Assert.AreEqual(DayOfWeek.sunday, Maybe.ofEnum<DayOfWeek>(0).get(DayOfWeek.tuesday));
    Assert.AreEqual(DayOfWeek.tuesday, Maybe.ofEnum<DayOfWeek>(2).get(DayOfWeek.sunday));
    Assert.AreEqual(DayOfWeek.wednesday, Maybe.ofEnum<DayOfWeek>(3).get(DayOfWeek.sunday));
    Assert.AreEqual(DayOfWeek.saturday, Maybe.ofEnum<DayOfWeek>(6).get(DayOfWeek.sunday));

    Assert.IsTrue(Maybe.ofEnum<DayOfWeek>(6).isPresent());
    Assert.IsFalse(Maybe.ofEnum<DayOfWeek>(-1).isPresent());
    Assert.IsFalse(Maybe.ofEnum<DayOfWeek>(7).isPresent());
    Assert.IsFalse(Maybe.ofEnum<DayOfWeek>(8).isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testDictionary()
  {
    Assert.AreEqual("Scotty", Maybe.of(crew, "Engineer").get(""));
    Assert.AreEqual("Uhura", Maybe.of(crew, "Lieutenant").get(""));
    Assert.IsTrue(Maybe.of(crew, "Engineer").isPresent());
    Assert.IsTrue(Maybe.of(crew, "Lieutenant").isPresent());
    Assert.IsFalse(Maybe.of(crew, "Ensign").isPresent());
    Assert.IsFalse(Maybe.of(crew, "Nurse").isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
