using System;
using System.Collections;
using System.Collections.Generic;

namespace KingdomsRebellion.AI {
	public class Heap<T> : IEnumerable<T> where T : IComparable<T> {

		T[] _items;
		int _count;

		#region public
		public Heap(int initialSize) {
			_items = new T[initialSize];
		}

		public void Push(T t) {
			if (_count >= _items.Length) {
				T[] temp = new T[_items.Length << 1];
				Array.Copy(_items, temp, _count);
				_items = temp;
			}

			_items[_count] = t;
			Up();
			++_count;
		}

		public T Pop() {
			if (_count == 0) {
				throw new InvalidOperationException("can't peek an empty heap");
			}

			T t = _items[0];
			--_count;
			_items[0] = _items[_count];
			Down();
			return t;
		}

		public T Peek() {
			if (_count == 0) {
				throw new InvalidOperationException("can't peek an empty heap");
			}
			return _items[0];
		}

		public bool IsEmpty() {
			return _count == 0;
		}

		public void Clear() {
			_count = 0;
			_items = new T[_items.Length]; // TEST
		}

		public IEnumerator<T> GetEnumerator() {
			for (int i = 0; i < _count; ++i) {
				yield return _items[i];
			}

		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion

		#region internal
		static int Parent(int index) {
			return (index - 1) >> 1;
		}
		static int Left(int index) {
			return (index << 1) + 1;
		}
		static int Right(int index) {
			return (index << 1) + 2;
		}

		void Up() {
			int p = _count;
			T item = _items[p];
			int par = Parent(p);
			while (par > -1 && item.CompareTo(_items[par]) < 0) {
				_items[p] = _items[par];
				p = par;
				par = Parent(p);
			}
			_items[p] = item;
		}

		void Down() {
			int index;
			int p = 0;
			T item = _items[p];
			while (true) {
				int left = Left(p);
				if (left >= _count) {
					break;
				}

				int right = Right(p);
				if (right >= _count) {
					index = left;
				} else {
					index = _items[left].CompareTo(_items[right]) < 0 ? left : right;
				}

				if (item.CompareTo(_items[index]) > 0) {
					_items[p] = _items[index];
					p = index;
				} else {
					break;
				}
			}
			_items[p] = item;
		}
		#endregion
	}
}