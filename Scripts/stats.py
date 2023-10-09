import os
import numpy as np
import re
from typing import List, Tuple, Dict
from itertools import pairwise

class Utils:
    @staticmethod
    def flatten(l: list[list[str]]) -> list[str]:
        def flatten_inner(inner_list: any) -> list[any]:
            if isinstance(inner_list, str) or not hasattr(inner_list, "__iter__") or len(inner_list) == 1:
                return [inner_list]
            
            res = []
            for item in inner_list:
                res += flatten_inner(item)

            return res

        return flatten_inner(l)

    @staticmethod
    def format_table(table:list[list[any]]):
        largest = len(max(Utils.flatten(table), key=lambda x: len(str(x))))
        table_str = ""

        for i, row in enumerate(table):
            row_str = ""
            for j, col in enumerate(row):
                row_str += f" {col:<{largest}} "

                if j == 0:
                    row_str += "|"

            table_str += row_str + "\n"

            if i == 0:
                table_str += "-" * len(row_str) + "\n"

        return table_str
    
    def bold(s: str) -> str:
        return f"\033[1m{s}\033[0m"
    


class LogFile:
    path: str
    line_pattern: re.Pattern[str]
    lines: List[Tuple[str, str]]

    keys: Dict[str, int]
    counts: Dict[str, int]
    transition_counts: np.ndarray

    def __init__(self, path: str) -> None:
        assert os.path.exists(path), "File not found"

        self.path = path
        self.line_pattern = re.compile("^(\[\d+\.\d+\])\s+(.*)$", flags=re.MULTILINE)

        with open(path, "r") as f:
            self.lines = list(map(self.__parse_line, f.readlines()))

        # TODO: make this below a func for an iterator
        # scan once to construct keys and count actions
        self.keys = {}
        self.counts = {}
        for _, action in self.lines:
            # construct keys in order of appearance
            if action not in self.keys:
                self.keys[action] = len(self.keys)
                self.counts[action] = 0
            
            # count actions
            self.counts[action] += 1
            
        # scan again to calculate markov transition probabilities
        self.transition_counts = np.zeros((len(self.keys), len(self.keys)))

        actions = map(lambda x: x[1], self.lines)
        for action1, action2 in pairwise(actions):
            self.transition_counts[self.keys[action1], self.keys[action2]] += 1

    def __parse_line(self, line: str) -> Tuple[str, str]:
        _match = self.line_pattern.match(line)
        timestamp, action = _match.group(1), _match.group(2)
        return timestamp, action
    
    def __get_transition_matrix_str(self, keys: List[str] | None = None) -> str:
        if keys is None:
            keys = list(self.keys.keys())
        assert set(keys).issubset(self.keys.keys()), "Invalid keys"

        # transition matrix
        float_to_str = lambda x: f"{x:.3f}"
        table = np.empty((len(keys) + 1, len(keys) + 1), dtype=object)

        table[0, 0] = ""
        table[0, 1:] = keys
        table[1:, 0] = keys
        
        table[1:, 1:] = self.transition_counts[[self.keys[k] for k in keys], :][:, [self.keys[k] for k in keys]]
        table[1:, 1:] /= table[1:, 1:].sum(axis=1)[:, np.newaxis]

        table[1:, 1:] = np.vectorize(float_to_str)(table[1:, 1:])
        transition_matrix = Utils.format_table(table.tolist())

        return transition_matrix
        
    def summary(self, transition_keys: List[str] | None = None) -> str:
        transition_matrix = self.__get_transition_matrix_str(transition_keys)
        counts = [
            ["Action"] + list(self.counts.keys()), 
            ["Count"] + list(self.counts.values())
        ]
        counts = np.array(counts).T.tolist()
        summary = f"""
{Utils.bold("Summary")}
Path: {self.path}

{Utils.bold("Counts")}
{Utils.format_table(counts)}

{Utils.bold("Transition Matrix")}
{transition_matrix}
        """
        return summary

def main(
    *,
    path: str,
):
    log_file_parser = LogFile(path)
    keys = ["W", "space", "shift down", "shift up"]
    print(log_file_parser.summary(keys))

import argparse
if __name__ == "__main__":
    parser = argparse.ArgumentParser("Stats tooling")
    parser.add_argument("path", type=str)
    args = parser.parse_args()
    main(
        path=args.path
    )