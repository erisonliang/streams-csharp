//======================================================================================================================
namespace hhlogic.streams {
//----------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
//======================================================================================================================


//======================================================================================================================
// Maybe<T> Builder
// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
public static class Maybe
{
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a maybe from a value.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Maybe<T> of<T>(T value)
  {
    return Maybe<T>.of(value);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Get a value from a dictionary using the given key.  Return nothing if there is no mapping.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Maybe<T> of<K, T>(Dictionary<K, T> map, K key)
  {
    T value;
    return map.TryGetValue(key, out value)? Maybe.of(value) : Maybe<T>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Convert a string to an int.  Return nothing if the string is not convertable.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Maybe<int> ofInt(string s)
  {
    int i;
    return int.TryParse(s, out i)? Maybe.of(i) : Maybe<int>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Convert a string to an unsigned int.  Return nothing if the string is not convertable.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Maybe<uint> ofUint(string s)
  {
    uint i;
    return uint.TryParse(s, out i)? Maybe.of(i) : Maybe<uint>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Convert an int to an enum of type T.
  // Return nothing if T is not an enum or T has no member with a value of the given int.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Maybe<T> ofEnum<T>(int i)
  {
    return Stream.ofEnum<T>().filter(e => i == (int)Convert.ChangeType(e, typeof(int))).first();
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
// A Monad containing a non-null value, or nothing.
// = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
[Serializable]
public struct Maybe<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  public static readonly Func<Maybe<T>, Maybe<T>> identity = m => m;
  public static readonly Maybe<T> nothing;
  //--------------------------------------------------------------------------------------------------------------------
  private bool present;
  private T value;
  //--------------------------------------------------------------------------------------------------------------------
  // Construct a Maybe from a value.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Maybe<T> of(T value)
  {
    if(value == null || value.Equals(null))
      return nothing;

    Maybe<T> m;
    m.present = true;
    m.value = value;
    return m;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Returns true if this contains a value, otherwise false.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public bool isPresent()
  {
    return present && !value.Equals(null);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Returns false if this contains a value, otherwise true.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public bool isNotPresent()
  {
    return !isPresent();
  }
  //--------------------------------------------------------------------------------------------------------------------
  // If this contains a value then invoke the given action providing the value.  Returns this.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> ifPresent(Action<T> a)
  {
    if(!isPresent())
      return nothing;

    a(value);
    return this;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // If this contains a value then invoke the given action.  Returns this.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> ifPresent(Action a)
  {
    return ifPresent(i => a());
  }
  //--------------------------------------------------------------------------------------------------------------------
  // If this does not contain a value then invoke the given action.  Returns this.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> ifNotPresent(Action a)
  {
    if(isPresent())
      return this;

    a();
    return nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return this if the value passes the given filter, otherwise return nothing.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> filter(Predicate<T> p)
  {
    return isPresent() && p(value)? this : nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return this if the provided filter returns true, otherwise return nothing.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> filter(Func<bool> f)
  {
    return filter(i => f());
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return a maybe containing a value provided by the given map function.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<M> map<M>(Func<T, M> f)
  {
    return isPresent()? Maybe<M>.of(f(value)) : Maybe<M>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return a maybe containing the value extracted from a maybe that was provided by the given map function.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<M> flatMap<M>(Func<T, Maybe<M>> f)
  {
    return isPresent()? f(value) : Maybe<M>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return a stream provided by the given map function.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Stream<M> flatMap<M>(Func<T, Stream<M>> f)
  {
    return isPresent()? f(value) : Stream.empty<M>();
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Combine to maybe objects if both are not nothing.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<Tuple<T, M>> join<M>(Maybe<M> that)
  {
    Maybe<T> thisMaybe = this;
    return this.isPresent() && that.isPresent()?
      that.map<Tuple<T, M>>(m => Tuple.of(thisMaybe.value, m)) : Maybe<Tuple<T, M>>.nothing;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return the value contained in this maybe if present, otherwise return the value provided by the given method.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public T get(Func<T> f)
  {
    return isPresent()? value : f();
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return the value contained in this maybe if present, otherwise return the value provided.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public T get(T t)
  {
    return isPresent()? value : t;
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return this if a value is present otherwise return a maybe containing the value provided by the given method.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> orElseOf(Func<T> f)
  {
    return isPresent()? this : Maybe.of(f());
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return this if a value is present otherwise return a maybe containing the value provided.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> orElseOf(T value)
  {
    return isPresent()? this : Maybe.of(value);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return this if a value is present
  // otherwise return a maybe containing the value contained in the maybe provided by the given function.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> orElseOfFlatMap(Func<Maybe<T>> f)
  {
    return isPresent()? this : f();
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Return this if a value is present otherwise return a maybe containing the value contained in the provided maybe.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public Maybe<T> orElseOfFlatMap(Maybe<T> value)
  {
    return isPresent()? this : value;
  }
  // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
