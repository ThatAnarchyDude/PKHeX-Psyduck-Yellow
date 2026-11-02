"""Decoder for the game's message banks (ROM a/0/2/7 files): returns (key, list of u16 word lists). Same routine as the Psyduck Yellow build tools."""
import struct


def decode_messages(data):
    count, key = struct.unpack_from('<HH', data)
    messages = []
    for i in range(count):
        mask = (765 * (i + 1) * key) & 65535
        mask |= mask << 16
        offset, length = struct.unpack_from('<II', data, 4 + i * 8)
        offset ^= mask
        length ^= mask
        assert offset + length * 2 <= len(data)
        seed = ((i + 1) * 596947) & 65535
        words = []
        for word in struct.unpack_from('<' + 'H' * length, data, offset):
            words.append(word ^ seed)
            seed = (seed + 18749) & 65535
        messages.append(words)
    return key, messages
