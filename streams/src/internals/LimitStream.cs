﻿//======================================================================================================================
namespace hhlogic.streams.internals {
//----------------------------------------------------------------------------------------------------------------------
using System;
//======================================================================================================================


//======================================================================================================================
public sealed class LimitStream<T> : AbstractStream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  private readonly Stream<T> underlyingStream;
  private readonly uint limitSize;
  //--------------------------------------------------------------------------------------------------------------------
  public LimitStream(Stream<T> underlyingStream, uint limitSize)
  {
    this.underlyingStream = underlyingStream;
    this.limitSize = limitSize;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override bool forEachWhile(Predicate<T> f)
  {
    uint i = 0u;

    underlyingStream.forEachWhile(t =>
    {
      if(i >= limitSize || !f(t))
        return false;

      i++;
      return true;
    });

    return i >= limitSize;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<uint> fastCount()
  {
    return underlyingStream.fastCount().map(i => limitSize < i? limitSize : i);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> last()
  {
    return reduce(Maybe<T>.nothing, (m, i) => Maybe.of(i));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> head()
  {
    return limitSize > 0? underlyingStream.head() : Maybe<T>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Stream<T> tail()
  {
    return limitSize > 1u?
      new LimitStream<T>(underlyingStream.tail(), limitSize - 1) as Stream<T> : EmptyStream<T>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
