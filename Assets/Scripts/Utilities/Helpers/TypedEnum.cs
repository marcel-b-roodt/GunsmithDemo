using System.Collections;

public class TypedEnum: IEnumerable
{
	public override string ToString()
	{
		return Value;
	}

	public IEnumerator GetEnumerator()
	{
		return ((IEnumerable)Value).GetEnumerator();
	}

	protected TypedEnum(string value)
	{
		this.Value = value;
	}

	public string Value { get; private set; }
}