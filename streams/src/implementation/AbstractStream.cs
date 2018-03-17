//======================================================================================================================
namespace hhlogic.streams.implementation {
//----------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
//======================================================================================================================


//======================================================================================================================
public abstract class AbstractStream<T> : Stream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  protected AbstractStream()
  {
  }
  //--------------------------------------------------------------------------------------------------------------------
  public static Stream<T> of(T value)
  {
    return value != null? new SingletonStream<T>(value) as Stream<T> : EmptyStream<T>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public static Stream<T> of(params T[] array)
  {
    return array != null? new ArrayStream<T>(array, 0u) as Stream<T> : EmptyStream<T>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public static Stream<int> ofRange(int rangeStartInclusive, int rangeEndExclusive)
  {
    if(rangeStartInclusive < rangeEndExclusive)
      return new IntStream(rangeStartInclusive, rangeEndExclusive);

    return EmptyStream<int>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public static Stream<uint> ofRange(uint rangeStartInclusive, uint rangeEndExclusive)
  {
    if(rangeStartInclusive < rangeEndExclusive)
      return new UintStream(rangeStartInclusive, rangeEndExclusive);

    return EmptyStream<uint>.instance;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public void forEach(Action<T> f)
  {
    forEachWhile(i =>
    {
      f(i);
      return true;
    });
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> ifPresent(Action<T> a)
  {
    return map(v =>
    {
      a(v);
      return v;
    });
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> filter(Predicate<T> p)
  {
    return new FilterStream<T>(this, p);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<M> map<M>(Func<T, M> f)
  {
    return new MapStream<T, M>(this, f);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<M> map<M>(uint mergeSize, Func<T[], M> mapper)
  {
    return new MergingMapStream<T, M>(this, mergeSize, mapper);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<M> mapIfPresent<M>(Func<T, Maybe<M>> mapper)
  {
    Func<Maybe<M>, M> f = m => m.get(() => {throw new Exception();});
    return map(i => mapper(i)).filter(m => m.isPresent()).map<M>(f);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<M> flatMap<M>(Func<T, Stream<M>> f)
  {
    return new FlatMapStream<T, M>(this, f);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public T[] toArray()
  {
    return fastCount()
      .map(c =>
      {
        uint i = 0;

        return reduce(new T[c], (a, t) =>
        {
          a[i++] = t;
          return a;
        });
      })
      .get(() =>
      {
        var list = reduce(new List<T>(), (a, t) =>
        {
          a.Add(t);
          return a;
        });

        return list.ToArray();
      });
  }
  //--------------------------------------------------------------------------------------------------------------------
  public List<T> toList()
  {
    return reduce(new List<T>(), (a, t) =>
    {
      a.Add(t);
      return a;
    });
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Dictionary<K, V> toDictionary<K, V>(Func<T, Entry<K, V>> entryGenerator)
  {
    return reduce(new Dictionary<K, V>(), (d, i) =>
    {
      var entry = entryGenerator(i);
      d.Add(entry.key, entry.value);
      return d;
    });
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Maybe<T> first()
  {
    return head();
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> limit(uint i)
  {
    return new LimitStream<T>(this, i);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> skip(uint i)
  {
    return new SkipStream<T>(this, i);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> append(T value)
  {
    return join(new SingletonStream<T>(value));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> prepend(T value)
  {
    return new PrependStream<T>(value, this);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> join(Stream<T> that)
  {
    return Stream.of(this, that).flatMap(s => s);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> join(params T[] array)
  {
    return join(of(array));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> repeat(uint factor)
  {
    return ofRange(0u, factor).flatMap(i => this);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<M> repeat<M>(params Func<T, M>[] maps)
  {
    return repeat(Stream.of(maps));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<M> repeat<M>(Stream<Func<T, M>> maps)
  {
    Func<Func<T, M>, Stream<M>> fmap = m => this.map(m);
    return maps.flatMap(fmap);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Maybe<T> reduce(Func<T, T, T> accumulator)
  {
    return head().map(h => tail().reduce(h, accumulator));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public U reduce<U>(U aggregate, Func<U, T, U> accumulator)
  {
    forEach(i => aggregate = accumulator(aggregate, i));
    return aggregate;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public U reduce<U>(hhlogic.streams.Tuple<U, Func<U, T, U>> reducer)
  {
    return reduce(reducer.first, reducer.second);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> reverse()
  {
    var array = toArray();
    Array.Reverse(array);
    return of(array);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> sort(Func<T, T, int> f)
  {
    var array = toArray();
    Array.Sort(array, new Sorter<T>(f));
    return Stream.of(array);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> shuffle()
  {
    return shuffle(new Random());
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> shuffle(Random random)
  {
    var list = new List<T>(toArray());

    var shuffled = Stream.ofRange(0, list.Count)
      .map(i => random.Next(0, list.Count))
      .map(r =>
      {
        var v = list[r];
        list.RemoveAt(r);
        return v;
      })
      .toArray();

    return new ArrayStream<T>(shuffled, 0u);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public Stream<T> distinct()
  {
    var distinctList = reduce(new List<T>(), (list, t) =>
    {
      if(!list.Contains(t))
        list.Add(t);

      return list;
    });

    return Stream.of<T>(distinctList);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public bool isNotEmpty()
  {
    return fastCount().map(i => i > 0).get(() => head().isPresent());
  }
  //--------------------------------------------------------------------------------------------------------------------
  public bool isEmpty()
  {
    return !isNotEmpty();
  }
  //--------------------------------------------------------------------------------------------------------------------
  public virtual Stream<T> snapshot()
  {
    return new ArrayStream<T>(this.toArray(), 0u);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public virtual uint count()
  {
    return fastCount().get(() => reduce(0u, (sum, i) => sum + 1u));
  }
  //--------------------------------------------------------------------------------------------------------------------
  public abstract Maybe<uint> fastCount();
  //--------------------------------------------------------------------------------------------------------------------
  public abstract Maybe<T> last();
  //--------------------------------------------------------------------------------------------------------------------
  public abstract Maybe<T> head();
  //--------------------------------------------------------------------------------------------------------------------
  public abstract Stream<T> tail();
  //--------------------------------------------------------------------------------------------------------------------
  public abstract bool forEachWhile(Predicate<T> f);
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
