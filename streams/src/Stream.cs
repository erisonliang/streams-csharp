//======================================================================================================================
namespace hhlogic.streams {
//----------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using hhlogic.streams.internals;
//======================================================================================================================


//======================================================================================================================
// A Stream of objects
// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
public interface Stream<T>
{
  // Invoke an action for each element in this stream.
  void forEach(Action<T> a);

  // Invoke an predicate for each element in this stream, stopping if the provided predicate returns false.
  // Returns true if all elements were given to the provided predicate, otherwise false.
  bool forEachWhile(Predicate<T> f);

  // Invoke an action for each element in this stream
  // only when a subsequent iteration action is called (forEach, reduce, toArray, etc...).
  // Returns this.
  Stream<T> ifPresent(Action<T> a);

  // Invoke a predicate for each element in this stream.
  // Returns a stream contining only the elements for which the predicate returned true.
  Stream<T> filter(Predicate<T> p);

  // Return a stream contining a mapped value for each element in this stream using the provided mapping lambda.
  Stream<M> map<M>(Func<T, M> f);

  // Return a stream contining a mapped value for each group elements in this stream using the provided mapping lambda.
  // Elements are grouped sequentially with each group containing the given number of elements.
  Stream<M> map<M>(uint mergeSize, Func<T[], M> mapper);

  // Return a stream contining a mapped value for each element in this stream using the provided mapping lambda.
  // The values in the returned stream are extracted from each maybe returned by the mapping function.
  // Maybes containing nothing are ignored.
  Stream<M> mapIfPresent<M>(Func<T, Maybe<M>> f);

  // Map each element to a stream and return a concatenation all the mapped streams.
  Stream<M> flatMap<M>(Func<T, Stream<M>> f);

  // Return the number of elements in this stream, iterating over the stream if necessary.
  uint count();

  // Return the number of elements in this stream
  // only if the element count can be calculated without iterating over the stream,
  // otherwise return nothing.
  Maybe<uint> fastCount();

  // Returns true if this stream contains one or more values, otherwise false.
  bool isEmpty();

  // Returns true if this stream contains no values, otherwise false.
  bool isNotEmpty();

  // Return an array containing all the elements in this stream.
  T[] toArray();

  // Return an list containing all the elements in this stream.
  List<T> toList();

  // Map each element in this stream to a key-value pair and return a dictionary containg all the pairs.
  Dictionary<K, V> toDictionary<K, V>(Func<T, Entry<K, V>> entryGenerator);

  // Return a stream containing all the elements of this stream in reverse order.
  Stream<T> reverse();

  // Return a stream containing all the elements of this stream sorted using the provided lambda.
  Stream<T> sort(Func<T, T, int> f);

  // Return a stream containing all the elements of this stream in random order.
  Stream<T> shuffle();

  // Return a stream containing all the elements of this stream in random order using the given random number generator.
  Stream<T> shuffle(Random random);

  // Return a stream containing all the unique elements in this stream.
  Stream<T> distinct();

  // Return the first element in this stream.  Returns nothing if this stream is empty.
  Maybe<T> first();

  // Return the last element in this stream.  Returns nothing if this stream is empty.
  Maybe<T> last();

  // Return the first element in this stream.  Returns nothing if this stream is empty.
  Maybe<T> head();

  // Return a stream containing all the elements in this stream except the first element.
  Stream<T> tail();

  // Return a stream containing the elements in this stream up to the given maximum number of elements.
  Stream<T> limit(uint i);

  // Return a stream containing the elements in this stream
  // excluding the given number of elements at the begining of this stream.
  Stream<T> skip(uint i);

  // Return a stream containing all the elements in this stream with one given additional element at the end.
  Stream<T> append(T value);

  // Return a stream containing all the elements in this stream with one given additional element at the begining.
  Stream<T> prepend(T value);

  // Return a stream containing all the elements in this stream and all the elements in the given stream.
  Stream<T> join(Stream<T> stream);

  // Return a stream containing all the elements in this stream and all the elements in the given array.
  Stream<T> join(params T[] array);

  // Return a stream the repeats all the elements in this stream the given number of times.
  Stream<T> repeat(uint factor);

  // Pass each element in this stream to each mapping function provided
  // and return a stream containing all of the resulting elements.
  Stream<M> repeat<M>(params Func<T, M>[] maps);

  // Pass each element in this stream to each mapping function provided
  // and return a stream containing all of the resulting elements.
  Stream<M> repeat<M>(Stream<Func<T, M>> maps);

  // Reduce the elements in the stream to a single value.  Returns nothing if this stream is empty.
  Maybe<T> reduce(Func<T, T, T> accumulator);

  // Reduce the elements in the stream to a single value.
  U reduce<U>(U identity, Func<U, T, U> accumulator);

  // Reduce the elements in the stream to a single value.
  U reduce<U>(Tuple<U, Func<U, T, U>> reducer);

  // Iterate over this stream and return a stream containg all the calculated values.
  Stream<T> snapshot();
}
//======================================================================================================================


//======================================================================================================================
// Stream Builder
// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
public static class Stream 
{
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing one value.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> of<T>(T value)
  {
    return AbstractStream<T>.of(value);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing zero or more value.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> of<T>(params T[] array)
  {
    return AbstractStream<T>.of(array);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing all the members of an Enumerable.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> of<T>(IEnumerable enumerable)
  {
    if(enumerable == null)
      return EmptyStream<T>.instance;

    var i = enumerable.GetEnumerator();
    return new SupplierStream<T>(() => i.MoveNext()? Maybe.of((T) i.Current) : Maybe<T>.nothing);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream that repeatedly invokes the given lambda for values until the lambda returns nothing.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> of<T>(Func<Maybe<T>> f)
  {
    return f != null? new SupplierStream<T>(f) as Stream<T> : EmptyStream<T>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream from a predicate that provides values.
  //
  // The provided predicate (lambda-A) will recieve a predicate as input (lambda-B).
  // lambda-A must repeatedly invoke lambda-B until lamda-A has no more values to provide or lambda-B returns false.
  // lambda-A must return true if all potential values were given to lamba-B, otherwise false.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> of<T>(Predicate<Predicate<T>> f)
  {
    return f != null? new IteratorStream<T>(f, 0u) as Stream<T> : EmptyStream<T>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing all int values between a start value (inclusive) and an end value (exclusive).
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<int> ofRange(int rangeStartInclusive, int rangeEndExclusive)
  {
    return AbstractStream<int>.ofRange(rangeStartInclusive, rangeEndExclusive);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing all uint values between a start value (inclusive) and an end value (exclusive).
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<uint> ofRange(uint rangeStartInclusive, uint rangeEndExclusive)
  {
    return AbstractStream<uint>.ofRange(rangeStartInclusive, rangeEndExclusive);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing all values contained in the provided maybe objects.
  // Maybes containing nothing are ignored.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> ofMaybe<T>(params Maybe<T>[] array)
  {
    Func<Maybe<T>, T> f = m => m.get(() => {throw new Exception();});
    return of<Maybe<T>>(array).filter(m => m.isPresent()).map(f);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a stream containing all the members of a given enum.  Returns an empty stream if T is not an emum.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> ofEnum<T>()
  {
    try
    {
      return of((T[]) Enum.GetValues(typeof(T)));
    }
    catch
    {
      return empty<T>();
    }
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Get an empty stream.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Stream<T> empty<T>()
  {
    return EmptyStream<T>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
