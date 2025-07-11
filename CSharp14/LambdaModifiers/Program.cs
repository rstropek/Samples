﻿// Before
TryParseCsv tryParseCsv = (string input, out List<int> result) => {
    result = [];
    foreach(var column in input.Split(',')) {
        if(int.TryParse(column, out int value)) {
            result.Add(value);
            return true;
        }
    }

    result.Clear();
    return false;
};

// New (note: you can leave out types for the parameters)
TryParseCsv tryParseCsvNew = (input, out result) => {
    result = [];
    foreach(var column in input.Split(',')) {
        if(int.TryParse(column, out int value)) {
            result.Add(value);
            return true;
        }
    }

    result.Clear();
    return false;
};

// Note: Works with other parameter modifiers, too (e.g. ref, in, out, scoped),
//       not for params modifier.

delegate bool TryParseCsv(string input, out List<int> result);