//======================================================================================================================
namespace hhlogic.streams.internals {
//----------------------------------------------------------------------------------------------------------------------
using System;
//======================================================================================================================


//======================================================================================================================
public sealed class FlatMapStream<U, T> : AbstractStream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  private readonly Stream<U> underlyingStream;
  private readonly Stream<T> headStream;
  private readonly Func<U, Stream<T>> mapper;
  //--------------------------------------------------------------------------------------------------------------------
  public FlatMapStream(Stream<U> underlyingStream, Func<U, Stream<T>> mapper)
  {
    this.underlyingStream = underlyingStream;
    this.headStream = EmptyStream<T>.instance;
    this.mapper = mapper;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public FlatMapStream(Stream<T> headStream, Stream<U> underlyingStream, Func<U, Stream<T>> mapper)
  {
    this.underlyingStream = underlyingStream;
    this.headStream = headStream;
    this.mapper = mapper;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override bool forEachWhile(Predicate<T> f)
  {
    return headStream.forEachWhile(f) && underlyingStream.forEachWhile(m => mapper(m).forEachWhile(f));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override uint count()
  {
    return headStream.count() + underlyingStream.reduce(0u, (sum, m) => sum + mapper(m).count());
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<uint> fastCount()
  {
    return Maybe<uint>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> last()
  {
    var l = underlyingStream.last().flatMap(mapper).last();
    return l.isPresent()? l : headStream.last();
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Maybe<T> head()
  {
    return next(headStream, underlyingStream).headStream.head();
  }
  //--------------------------------------------------------------------------------------------------------------------
  public override Stream<T> tail()
  {
    var n = next(headStream, underlyingStream);
    return next(n.headStream.tail(), n.underlyingStream);
  }
  //--------------------------------------------------------------------------------------------------------------------
  private FlatMapStream<U, T> next(Stream<T> headStream, Stream<U> underlying)
  {
    var h = headStream.head();

    if(h.isPresent())
      return new FlatMapStream<U, T>(headStream, underlying, mapper);

    var uh = underlying.head();

    return uh.isNotPresent()? new FlatMapStream<U, T>(EmptyStream<T>.instance, EmptyStream<U>.instance, mapper)
        : next(uh.flatMap(mapper), underlying.tail());
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
