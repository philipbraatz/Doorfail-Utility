﻿using System.Text;

public class StringCompressor
{
    private const string CHARACTER_SET = " abcdefghijklmnopqrstuvwxyz.-#";

    public static byte[] Compress(string input)
    {
        if(string.IsNullOrEmpty(input) || input.Length < 4)
        { return input?.ToCharArray().Select(c => (byte)c).ToArray() ?? []; }

        byte[] halfBytes = SplitStringsIntoHalfBytes(input);
        byte[] compressed = CompressHalfBytes(halfBytes);

        return compressed;
    }

    public static string TestSpliting(string input)
    {
        byte[] halfBytes = SplitStringsIntoHalfBytes(input);
        return MergeHalfBytes(halfBytes);
    }

    public static string TestCompressing(string input)
    {
        byte[] halfBytes = SplitStringsIntoHalfBytes(input);
        byte[] compressed = CompressHalfBytes(halfBytes);
        var decompressed = DecompressHalfBytes(compressed);
        return MergeHalfBytes(decompressed);
    }
    public static string Decompress(byte[] input)
    {
        if(input.Length < 4)
        { return new string(input.Select(c => (char)c).ToArray()); }

        byte[] halfBytes = DecompressHalfBytes(input);
        return MergeHalfBytes(halfBytes);
    }

    private static byte[] SplitStringsIntoHalfBytes(string input)
    {
        // Convert to lowercase and remove anything that's not a-z or space
        string cleanedInput = new string(input
            .ToLower()
            .Where(c => c is>='a' and <='z'or ' ' or '.' or'-' or'#')
            .ToArray());

        List<byte> bytes = new List<byte>();
        foreach(char c in cleanedInput)
        {
            var num = GetHalfBytes(c);
            bytes.Add(num.Item1);
            bytes.Add(num.Item2);
        }

        // If the function doesn't return a multiple of 4, add 2 empty bytes
        while(bytes.Count % 4 != 0)
        {
            bytes.Add(0);
        }

        return [.. bytes];
    }

    private static (byte, byte) GetHalfBytes(char c)
    {
        if(CHARACTER_SET.Length >= 31)
            throw new ArgumentException("Invalid validCharacters string: it must contain fewer than 31 characters.");

        byte num;
        int index = CHARACTER_SET.IndexOf(c);

        if(index != -1)
        {
            num = (byte)(index + 1); // Map characters based on their position in the string
        } else
        {
            // If the character is not in the valid characters, return a default value
            num = (byte)CHARACTER_SET.IndexOf('.');
        }

        // Split the number into two parts using bit shifting
        var part1 = (byte)((num & 0b111) << 2);   // Get the first 3 bits
        var part2 = (byte)((num & 0b11000) >> 3); // Get the last 2 bits

        return (part1, part2);
    }

    private static char GetCharacter(byte part1, byte part2)
    {
        // Combine the two half bytes into a single number
        byte num = (byte)(((part1 >> 2) & 0b00000111) | ((part2 << 3) & 0b00011000));
        if(num < 1 || num > CHARACTER_SET.Length)
            return '?';

        return CHARACTER_SET[num - 1]; 
    }

    private static byte[] CompressHalfBytes(byte[] input)
    {
        // Pad the input so it's divisible by 3
        int padding = input.Length % 4;
        if(padding != 0)
        {
            Array.Resize(ref input, input.Length + (4 - padding));
        }

        List<byte> output = new List<byte>();

        for(int i = 0; i < input.Length; i += 4)
        {
            // Variables for current set of bytes
            byte even1 = input[i];
            byte odd1 = input[i + 1];
            byte even2 = input[i + 2];
            byte odd2 = input[i + 3];

            // Generate byte1: Combination of the first two even bytes
            var byte1 = (byte)(((even1 & 0b111) << 2) | ((even2 & 0b110) >> 1));

            // Generate byte2: Combination of leftover bits from odd bytes of the first two and the first even byte of the next set
            var byte2 = (byte)(((even2 & 0b001) << 5) | ((odd1 & 0b111) << 2) | ((even2 & 0b100) >> 1));

            // Generate byte3: Combination of the last even byte from the current set and the remaining bits from odd bytes of the current and next set
            var byte3 = (byte)(((even2 & 0b011) << 4) | ((odd2 & 0b110) << 1) | ((odd2 & 0b001) >> 2));

            // Add the generated bytes to the output list
            output.Add(byte1);
            output.Add(byte2);
            output.Add(byte3);
        }

        return output.ToArray();
    }


    private static byte[] DecompressHalfBytes(byte[] input)
    {
        // Pad the input so it's divisible by 3
        int padding = input.Length % 3;
        if(padding != 0)
        {
            Array.Resize(ref input, input.Length + (3 - padding));
        }

        List<byte> output = [];

        for(int i = 0; i < input.Length; i += 3)
        {
            byte byte1 = input[i];
            byte byte2 = input[i + 1];
            byte byte3 = input[i + 2];

            // Reconstruct even1 and even2 from byte1
            byte even1 = (byte)((byte1 & 0b01110000) >> 4);
            byte even2 = (byte)(byte1 & 0b0000111);

            // Reconstruct odd1, odd2, and even3 from byte2
            byte odd1 = (byte)((byte2 & 0b11000000) >> 6 );
            byte odd2 = (byte)((byte2 & 0b00110000) >> 4 );
            byte even3 = (byte)(byte2 & 0b00000111);

            // Reconstruct even4, odd3, and odd4 from byte3
            byte even4 = (byte)((byte3 & 0b11100000) >> 5);
            byte odd3 = (byte)((byte3 & 0b00001100) >> 2);
            byte odd4 = (byte)(byte3 & 0b00000011);

            // Add the reconstructed bytes to the output list
            output.Add(even1);
            output.Add(odd1);
            output.Add(even2);
            output.Add(odd2);
            output.Add(even3);
            output.Add(even4);
            output.Add(odd3);
            output.Add(odd4);
        }

        // Trim any padding bytes
        while(output.Count > 0 && output[^1] == 0)
        {
            output.RemoveAt(output.Count - 1);
        }

        return [.. output];
    }

    private static string MergeHalfBytes(byte[] input)
    {
        int padding = input.Length % 3;
        if(padding != 0)
        {
            Array.Resize(ref input, input.Length + (3 - padding));
        }

        StringBuilder sb = new();
        int length = input.Length;

        // Process the input bytes in pairs
        for(int i = 0; i < length - 1; i += 2)
        {
            sb.Append(GetCharacter(input[i], input[i + 1]));
        }

        return sb.ToString();
    }
}
