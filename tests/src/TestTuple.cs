//======================================================================================================================
namespace hhlogic.streams.test {
//----------------------------------------------------------------------------------------------------------------------
using System;
using hhlogic.streams;

#if __MonoCS__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using CollectionAssert = NUnit.Framework.Assert;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Tuple = hhlogic.streams.Tuple;
//======================================================================================================================


//======================================================================================================================
[TestClass]
public class TestTuple
{
  //--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testTuple()
  {
    var t1 = Tuple.of(1);
    Assert.AreEqual(1, t1.first);

    var t2 = Tuple.of(1, "a");
    Assert.AreEqual(1, t2.first);
    Assert.AreEqual("a", t2.second);

    var t3 = Tuple.of(1, "a", DayOfWeek.sunday);
    Assert.AreEqual(1, t3.first);
    Assert.AreEqual("a", t3.second);
    Assert.AreEqual(DayOfWeek.sunday, t3.third);

    var t4 = Tuple.of(1, "a", DayOfWeek.sunday, true);
    Assert.AreEqual(1, t4.first);
    Assert.AreEqual("a", t4.second);
    Assert.AreEqual(DayOfWeek.sunday, t4.third);
    Assert.AreEqual(true, t4.forth);
  }
//--------------------------------------------------------------------------------------------------------------------
  [TestMethod]
  public void testAppend()
  {
    var t1 = Tuple.of(1);

    var t2 = t1.append("a");
    Assert.AreEqual(1, t2.first);
    Assert.AreEqual("a", t2.second);

    var t3 = t2.append(DayOfWeek.sunday);
    Assert.AreEqual(1, t3.first);
    Assert.AreEqual("a", t3.second);
    Assert.AreEqual(DayOfWeek.sunday, t3.third);

    var t4 = t3.append(true);
    Assert.AreEqual(1, t4.first);
    Assert.AreEqual("a", t4.second);
    Assert.AreEqual(DayOfWeek.sunday, t4.third);
    Assert.AreEqual(true, t4.forth);
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
