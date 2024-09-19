namespace Tetraizor.Utils;

using System;

public class ReactiveProperty<T> : IEquatable<ReactiveProperty<T>>
{
    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                T oldValue = _value;
                _value = value;

                ValueChanged?.Invoke(oldValue, _value);
            }
        }
    }

    public delegate void ValueChangedEventHandler(T oldValue, T newValue);
    public event ValueChangedEventHandler ValueChanged;

    public ReactiveProperty(T initialValue = default)
    {
        _value = initialValue;
    }

    public override string ToString()
    {
        return _value?.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is ReactiveProperty<T> reactiveProperty)
        {
            return Equals(reactiveProperty);
        }

        return false;
    }

    public bool Equals(ReactiveProperty<T> other)
    {
        return _value.Equals(other._value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static implicit operator T(ReactiveProperty<T> reactiveProperty)
    {
        return reactiveProperty.Value;
    }

    public static implicit operator ReactiveProperty<T>(T value)
    {
        return new ReactiveProperty<T>(value);
    }

    #region Equal Operators
    public static bool operator ==(ReactiveProperty<T> left, T right)
    {
        return left.Value.Equals(right);
    }

    public static bool operator !=(ReactiveProperty<T> left, T right)
    {
        return !left.Value.Equals(right);
    }

    public static bool operator ==(T left, ReactiveProperty<T> right)
    {
        return left.Equals(right.Value);
    }

    public static bool operator !=(T left, ReactiveProperty<T> right)
    {
        return !left.Equals(right.Value);
    }

    public static bool operator ==(ReactiveProperty<T> left, ReactiveProperty<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ReactiveProperty<T> left, ReactiveProperty<T> right)
    {
        return !left.Equals(right);
    }
    #endregion
}