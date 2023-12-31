﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MatrixWeb.Extensions;
public static class ExtensionsMethods {
    public static async Task<TResult?> RetryAsync<TResult>(this Func<Task<TResult?>> createTask, int maxRetries, ILogger logger) {
        TResult? response = default;
        Exception? exception = null;
        int retries = 0;

        while (retries < maxRetries) {
            try {
                response = await createTask();
                break;
            } catch (Exception ex) {
                exception = ex;
                retries++;
            }
        }

        if (retries >= maxRetries) {
            logger.LogError("Couldn't get the data status in 5 tries!");
            logger.LogError("Last Exception: {exception}", exception);
            return default;
        }

        if (retries > 0) {
            logger.LogWarning("{retries} tries needed to get the current data", retries + 1);
        }


        return response;
    }

    public static bool TryFirst<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, out T? value) {
        IEnumerator<T> enumerator = enumerable.Where(item => predicate(item)).GetEnumerator();
        if(enumerator.MoveNext()) {
            value = enumerator.Current;
            return true;
        }

        value = default;
        return false;
    }
}
