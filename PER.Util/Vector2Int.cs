﻿using System;

namespace PER.Util {
    public readonly struct Vector2Int {
        public readonly int x;
        public readonly int y;

        public Vector2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int operator +(Vector2Int left, Vector2Int right) =>
            new(left.x + right.x, left.y + right.y);

        public static Vector2Int operator -(Vector2Int left, Vector2Int right) =>
            new(left.x - right.x, left.y - right.y);

        public static Vector2Int operator *(Vector2Int left, int right) =>
            new(left.x * right, left.y * right);

        public static Vector2Int operator *(int left, Vector2Int right) => right * left;

        public static Vector2Int operator /(Vector2Int left, int right) =>
            new(left.x / right, left.y / right);

        public bool Equals(Vector2Int other) => x == other.x && y == other.y;

        public override bool Equals(object obj) => obj is Vector2Int other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(x, y);

        public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);
        public static bool operator !=(Vector2Int left, Vector2Int right) => !left.Equals(right);
    }
}
