using System;
using System.Threading.Tasks;

public class ProcessState<T1, T2>
{
    public T1 Source;
    public T2 Target;
    public Action<T1, T2> Act;
    public Func<T1, T2, Task<bool>> Func;
}