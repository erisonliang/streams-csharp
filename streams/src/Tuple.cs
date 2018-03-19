//======================================================================================================================
namespace hhlogic.streams {
//======================================================================================================================


//======================================================================================================================
public interface Tuple<A> {A first {get;} Tuple<A, B> append<B>(B v);}
public interface Tuple<A, B> : Tuple<A> {B second {get;} new Tuple<A, B, C> append<C>(C v);}
public interface Tuple<A, B, C> : Tuple<A, B> {C third {get;} new Tuple<A, B, C, D> append<D>(D v);}
public interface Tuple<A, B, C, D> : Tuple<A, B, C> {D forth {get;}}
//======================================================================================================================


//======================================================================================================================
// Tuple Builder
//======================================================================================================================
public static class Tuple
{
  //--------------------------------------------------------------------------------------------------------------------
  // Build a tuple containing two values.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Tuple<A, B> of<A, B>(A a, B b)
  {
    return new Double<A, B>(a, b);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Build a tuple containing three values.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Tuple<A, B, C> of<A, B, C>(A a, B b, C c)
  {
    return new Triple<A, B, C>(a, b, c);
  }
  //--------------------------------------------------------------------------------------------------------------------
  // Build a tuple containing four values.
  // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
  public static Tuple<A, B, C, D> of<A, B, C, D>(A a, B b, C c, D d)
  {
    return new Quadruple<A, B, C, D>(a, b, c, d);
  }
  //--------------------------------------------------------------------------------------------------------------------

  // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
  public class Single<T> : Tuple<T>
  {
    public readonly T value;
    public Single(T value) {this.value = value;}
    public T first {get {return value;}}
    public Tuple<T, U> append<U>(U u) {return Tuple.of(value, u);}
  }
  // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
  public class Double<A, T> : Single<A>, Tuple<A, T>
  {
    private new readonly T value;
    public Double(A a, T value) : base(a) {this.value = value;}
    public T second {get {return value;}}
    public new Tuple<A, T, U> append<U>(U u) {return Tuple.of(first, value, u);}
  }
  // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
  public class Triple<A, B, T> : Double<A, B>, Tuple<A, B, T>
  {
    private new readonly T value;
    public Triple(A a, B b, T value) : base(a, b) {this.value = value;}
    public T third {get {return value;}}
    public new Tuple<A, B, T, U> append<U>(U u) {return Tuple.of(first, second, value, u);}
  }
  // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
  public class Quadruple<A, B, C, T> : Triple<A, B, C>, Tuple<A, B, C, T>
  {
    private new readonly T value;
    public Quadruple(A a, B b, C c, T value) : base(a, b, c) {this.value = value;}
    public T forth {get {return value;}}
  }
  // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================
