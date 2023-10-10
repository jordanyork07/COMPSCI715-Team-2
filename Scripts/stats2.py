from matplotlib import pyplot as plt
import numpy as np
import pandas as pd
pd.options.mode.chained_assignment = None  # default='warn'




def plot_bar_chart(data: np.ndarray, labels: list[str], title: str, xlabel: str, ylabel: str, filename: str):
    x = np.arange(len(labels))
    width = 0.35

    fig, ax = plt.subplots()
    rects = ax.bar(x, data, width)

    ax.set_ylabel(ylabel)
    ax.set_xlabel(xlabel)
    ax.set_title(title)
    ax.set_xticks(x, labels, rotation="vertical")
    

    fig.tight_layout()
    fig.set_figheight(6)
    fig.set_figwidth(12)
    plt.savefig(filename)

def plot_line_chart(data: np.ndarray, labels: list[str], title: str, xlabel: str, ylabel: str, filename: str):  
    x = np.arange(len(labels))

    fig, ax = plt.subplots()
    ax.plot(x, data)

    ax.set_ylabel(ylabel)
    ax.set_xlabel(xlabel)
    ax.set_title(title)
    ax.set_xticks(x)
    ax.set_xticklabels(labels)

    # fig.tight_layout()
    
    plt.savefig(filename)

def process_possible_range(s: str) -> int:
    try:
        return float(s)
    except:
        if '-' in s:
            left, right = s.split('-')
            return (int(left) + int(right)) // 2
        else:
            raise ValueError(f"Invalid value: {s}")

def main(path):
    df = pd.read_csv(path)

    # get_positive_columns = lambda df, _type: df.columns[df.columns.str.contains(_type) and not df.columns.str.contains("Unreversed")]
    get_positive_columns = lambda df, _type: df.columns[df.columns.str.contains(_type) & ~df.columns.str.contains("Unreversed")]
    avg_swing = df[get_positive_columns(df, "SWING")].mean(axis=1)
    avg_random = df[get_positive_columns(df, "RANDOM")].mean(axis=1)
    avg_manual = df[get_positive_columns(df, "MANUAL")].mean(axis=1)

    df2 = pd.DataFrame({
        "SWING": avg_swing,
        "RANDOM": avg_random,
        "MANUAL": avg_manual
    })  
    print(df2.describe())
    # print(get_positive_columns(df, "SWING"))
    # number of hours vs fav type
    col_num_hours = 'Approximately how many hours a week do you spend playing video games?'
    col_fav_type = 'Which types of games do you prefer to play?'

    data = df[[col_num_hours, col_fav_type]]
    data[col_num_hours] = data[col_num_hours].apply(process_possible_range)     
    data.dropna(inplace=True)

    # split fav type into multiple columns
    data[col_fav_type] = data[col_fav_type].apply(lambda x: str(x).split(','))
    
    fav_types = set()
    for fav_type in data[col_fav_type]:
        fav_types.update(fav_type)

    for fav_type in fav_types:
        data[fav_type] = data[col_fav_type].apply(lambda x: fav_type in x)

    data = data.drop(col_fav_type, axis=1)

    types = list(fav_types)
    hours_per_type = np.zeros(len(types))

    for i, fav_type in enumerate(types):
        hours_per_type[i] = data[data[fav_type]][col_num_hours].mean()
    
    
    plot_bar_chart(hours_per_type, types, "Average number of hours spent playing ANY video games per week by favourite genres (non-exclusive)", "Favourite types of games", "Number of hours", "number_of_hours.png")

import argparse
if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("path", type=str)
    args = parser.parse_args()
    main(args.path)