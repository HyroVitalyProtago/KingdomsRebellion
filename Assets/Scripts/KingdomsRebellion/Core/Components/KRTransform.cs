using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Components {

    public class KRTransform : KRBehaviour {
    	public int __playerID, __sizeX, __sizeY; // just for Unity Editor

		Vec2 _pos;
		public Vec2 Pos {
			get {
				return _pos;
			}
			set {
				_pos = value;
				transform.position = _pos.ToVector3().Adjusted();
			}
		}
		public Vec2 Size { get; private set; }
		public int PlayerID { get; private set; }

		void Awake() {
			PlayerID = __playerID;
			Pos = Vec2.FromVector3(transform.position);
			Size = new Vec2(__sizeX, __sizeY);

			KRFacade.GetMap().Add(gameObject);
		}
    }
}