using System;

public interface IHasGameFrame {
	void GameFrameTurn(int gameFramesPerSecond);
	bool Finished { get; }
}
