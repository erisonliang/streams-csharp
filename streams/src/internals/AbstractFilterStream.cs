﻿//======================================================================================================================
namespace hhlogic.streams.internals {
//======================================================================================================================


//======================================================================================================================
public abstract class AbstractFilterStream<T> : AbstractStream<T>
{
  //--------------------------------------------------------------------------------------------------------------------
  protected AbstractFilterStream()
  {
  }
  //--------------------------------------------------------------------------------------------------------------------
  public sealed override Maybe<T> last()
  {
    return reduce(Maybe<T>.nothing, (m, i) => Maybe.of(i));
  }
  //--------------------------------------------------------------------------------------------------------------------
}
//======================================================================================================================


//======================================================================================================================
} // End Namespace
//======================================================================================================================