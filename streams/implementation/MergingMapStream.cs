//======================================================================================================================
namespace hhlogic.streams.implementation {
//----------------------------------------------------------------------------------------------------------------------
using System;
//======================================================================================================================


//======================================================================================================================
public sealed class MergingMapStream<U, T> : AbstractFilterStream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  private readonly Stream<U> underlyingStream;
  private readonly uint mergeSize;
  private readonly Func<U[], T> mapper;
  //--------------------------------------------------------------------------------------------------------------------
  public MergingMapStream(Stream<U> underlyingStream, uint mergeSize, Func<U[], T> mapper)
  {
    this.underlyingStream = underlyingStream;
    this.mapper = mapper;
    this.mergeSize = mergeSize;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override bool forEachWhile(Predicate<T> f)
  {
    if(mergeSize < 1u)
      return true;

    var values = new U[mergeSize];
    uint i = 0u;

    return underlyingStream.forEachWhile(m =>
    {
      values[i++] = m;

      if(i < mergeSize)
        return true;

      i = 0;
      return f(mapper(values));
    });
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<uint> fastCount()
  {
    return underlyingStream.fastCount().map(i => i/mergeSize);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> head()
  {
    var values = underlyingStream.limit(mergeSize).toArray();
    return values.Length == mergeSize? Maybe.of(mapper(values)) : Maybe<T>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Stream<T> tail()
  {
    return next(mergeSize, underlyingStream.tail());
  }
  //--------------------------------------------------------------------------------------------------------------------
  private Stream<T> next(uint i, Stream<U> underlying)
  {
    return i > 1u? next(i - 1, underlying.tail()) : new MergingMapStream<U, T>(underlying, mergeSize, mapper);
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
