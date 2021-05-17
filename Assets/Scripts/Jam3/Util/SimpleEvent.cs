using UnityEngine;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Simple event delegate.
    /// </summary>
    public delegate void SimpleEvent();

    /// <summary>
    /// Simple event delegate.
    /// </summary>
    /// <typeparam name="T0">The type of the 0.</typeparam>
    /// <param name="arg0">The arg0.</param>
    public delegate void SimpleEvent<T0>(T0 arg0);

    /// <summary>
    /// Simple event delegate.
    /// </summary>
    /// <typeparam name="T0">The type of the 0.</typeparam>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    public delegate void SimpleEvent<T0, T1>(T0 arg0, T1 arg1);

    /// <summary>
    /// Simple event delegate.
    /// </summary>
    /// <typeparam name="T0">The type of the 0.</typeparam>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    public delegate void SimpleEvent<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

    /// <summary>
    /// Simple event delegate.
    /// </summary>
    /// <typeparam name="T0">The type of the 0.</typeparam>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <param name="arg3">The arg3.</param>
    public delegate void SimpleEvent<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Simple event delegate.
    /// </summary>
    /// <typeparam name="T0">The type of the 0.</typeparam>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <param name="arg3">The arg3.</param>
    /// <param name="arg4">The arg4.</param>
    public delegate void SimpleEvent<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
