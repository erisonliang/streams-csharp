﻿//======================================================================================================================
namespace hhlogic.streams.internals {
//----------------------------------------------------------------------------------------------------------------------
using System;
//======================================================================================================================


//======================================================================================================================
public sealed class PrependStream<T> : AbstractStream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  private readonly T value;
  private readonly Stream<T> next;
  //--------------------------------------------------------------------------------------------------------------------
  public PrependStream(T value, Stream<T> next)
  {
    this.value = value;
    this.next = next;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override bool forEachWhile(Predicate<T> f)
  {
    return f(value)? next.forEachWhile(f) : false;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<uint> fastCount()
  {
    return next.fastCount().map(i => i + 1);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> last()
  {
    return next.last().orElseOf(value);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> head()
  {
    return Maybe.of(value);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Stream<T> tail()
  {
    return next;
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
