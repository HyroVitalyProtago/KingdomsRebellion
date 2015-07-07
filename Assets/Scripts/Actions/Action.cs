using System;

public abstract class Action {
	public int NetworkAverage { get; set; }
	public int RuntimeAverage { get; set; }
	public abstract void ProcessAction();
}