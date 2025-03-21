using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

namespace NGDtuanh.Utils {
    public static class RandomExtensions {
        public static ref Hash128 Random(this ref Hash128 hash) {
            Span<byte> bytes = stackalloc byte[16];
            RandomNumberGenerator.Fill(bytes);
            hash = new Hash128(
                MemoryMarshal.Read<ulong>(bytes[..8])
              , MemoryMarshal.Read<ulong>(bytes[8..]));
            return ref hash;
        }
        
        public static ref ulong Random(this ref ulong hash) {
            Span<byte> bytes = stackalloc byte[8];
            RandomNumberGenerator.Fill(bytes);
            hash = MemoryMarshal.Read<ulong>(bytes);
            return ref hash;
        }
        
        public static ref long Random(this ref long hash) {
            Span<byte> bytes = stackalloc byte[8];
            RandomNumberGenerator.Fill(bytes);
            hash = MemoryMarshal.Read<long>(bytes);
            return ref hash;
        }

        public static ref uint Random(this ref uint hash) {
            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.Fill(bytes);
            hash = MemoryMarshal.Read<uint>(bytes);
            return ref hash;
        }
        
        public static ref int Random(this ref int hash) {
            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.Fill(bytes);
            hash = MemoryMarshal.Read<int>(bytes);
            return ref hash;
        }
        
        public static ref ushort Random(this ref ushort hash) {
            Span<byte> bytes = stackalloc byte[2];
            RandomNumberGenerator.Fill(bytes);
            hash = MemoryMarshal.Read<ushort>(bytes);
            return ref hash;
        }
        
        public static ref short Random(this ref short hash) {
            Span<byte> bytes = stackalloc byte[2];
            RandomNumberGenerator.Fill(bytes);
            hash = MemoryMarshal.Read<short>(bytes);
            return ref hash;
        }
        
        public static ref byte Random(this ref byte hash) {
            Span<byte> bytes = stackalloc byte[1];
            RandomNumberGenerator.Fill(bytes);
            hash = bytes[0];
            return ref hash;
        }
    }
}