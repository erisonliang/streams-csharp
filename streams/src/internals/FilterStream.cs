//======================================================================================================================
namespace hhlogic.streams.internals {
//----------------------------------------------------------------------------------------------------------------------
using System;
//======================================================================================================================


//======================================================================================================================
public sealed class FilterStream<T> : AbstractFilterStream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  private readonly Stream<T> underlyingStream;
  private readonly Predicate<T> streamFilter;
  //--------------------------------------------------------------------------------------------------------------------
  public FilterStream(Stream<T> underlyingStream, Predicate<T> streamFilter)
  {
    this.underlyingStream = underlyingStream;
    this.streamFilter = streamFilter;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override bool forEachWhile(Predicate<T> f)
  {
    return underlyingStream.forEachWhile(i => streamFilter(i)? f(i) : true);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> head()
  {
    return nextHead(underlyingStream);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Stream<T> tail()
  {
    return new FilterStream<T>(nextTail(underlyingStream), streamFilter);
  }
  //--------------------------------------------------------------------------------------------------------------------
  private Maybe<T> nextHead(Stream<T> underlying)
  {
    var h = underlying.head();

    if(h.isNotPresent())
      return Maybe<T>.nothing;

    return h.filter(streamFilter).isPresent()? h : nextHead(underlying.tail());
  }
  //--------------------------------------------------------------------------------------------------------------------
  private Stream<T> nextTail(Stream<T> underlying)
  {
    var h = underlying.head();

    if(h.isNotPresent())
      return Stream.empty<T>();

    Stream<T> t = underlying.tail();
    return h.filter(streamFilter).isPresent()? t : nextTail(t);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<uint> fastCount()
  {
    return Maybe<uint>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
