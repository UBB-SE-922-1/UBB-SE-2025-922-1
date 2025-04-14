using System;
using System.Collections.Generic;
using System.Data;
using Duo.Interfaces;
using Microsoft.Data.SqlClient;
using Duo.Data;

namespace TestsDuo2.Mocks
{
    public class MockDataLink : IDataLink
    {
        private Dictionary<string, DataTable> _readerResponses = new Dictionary<string, DataTable>();
        private Dictionary<string, object?> _scalarResponses = new Dictionary<string, object?>();
        private Dictionary<string, int> _nonQueryResponses = new Dictionary<string, int>();
        
        private Dictionary<string, int> _readerCalls = new Dictionary<string, int>();
        private Dictionary<string, int> _scalarCalls = new Dictionary<string, int>();
        private Dictionary<string, int> _nonQueryCalls = new Dictionary<string, int>();

        public void OpenConnection()
        {
            // No-op for mock
        }
        public void CloseConnection()
        {
            // No-op for mock
        }

        public void SetupExecuteReaderResponse(string storedProcedureName, DataTable result)
        {
            _readerResponses[storedProcedureName] = result;
        }

        public void SetupExecuteReaderResponse(string storedProcedureName, SqlParameter[]? parameters, DataTable result)
        {
            string key = parameters != null ? GetKey(storedProcedureName, parameters) : storedProcedureName;
            _readerResponses[key] = result;
        }

        public void SetupExecuteScalarResponse<T>(string storedProcedureName, T result)
        {
            _scalarResponses[storedProcedureName] = result;
        }

        public void SetupExecuteScalarResponse<T>(string storedProcedureName, SqlParameter[]? parameters, T result)
        {
            string key = parameters != null ? GetKey(storedProcedureName, parameters) : storedProcedureName;
            _scalarResponses[key] = result;
        }

        public void SetupExecuteNonQueryResponse(string storedProcedureName, int result)
        {
            _nonQueryResponses[storedProcedureName] = result;
        }

        public void SetupExecuteNonQueryResponse(string storedProcedureName, SqlParameter[]? parameters, int result)
        {
            string key = parameters != null ? GetKey(storedProcedureName, parameters) : storedProcedureName;
            _nonQueryResponses[key] = result;
        }

        public DataTable ExecuteReader(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            string key = parameters != null && parameters.Length > 0 ? GetKey(storedProcedureName, parameters) : storedProcedureName;
            
            if (!_readerCalls.ContainsKey(storedProcedureName))
                _readerCalls[storedProcedureName] = 0;
            
            _readerCalls[storedProcedureName]++;
            
            return _readerResponses.ContainsKey(key) ? _readerResponses[key] : new DataTable();
        }

        public T ExecuteScalar<T>(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            string key = parameters != null && parameters.Length > 0 ? GetKey(storedProcedureName, parameters) : storedProcedureName;

            if (!_scalarCalls.ContainsKey(storedProcedureName))
                _scalarCalls[storedProcedureName] = 0;
            
            _scalarCalls[storedProcedureName]++;
            
            if (_scalarResponses.ContainsKey(key) && _scalarResponses[key] != null)
            {
                if (_scalarResponses[key] is T result)
                {
                    return result;
                }
                
                return (T)Convert.ChangeType(_scalarResponses[key]!, typeof(T));
            }
            
            // Special handling for int type when the result is null
            if (typeof(T) == typeof(int))
            {
                return (T)(object)-1;
            }
            
            return default!;
        }

        public int ExecuteNonQuery(string storedProcedureName, SqlParameter[]? parameters = null)
        {
            string key = parameters != null && parameters.Length > 0 ? GetKey(storedProcedureName, parameters) : storedProcedureName;

            if (!_nonQueryCalls.ContainsKey(storedProcedureName))
                _nonQueryCalls[storedProcedureName] = 0;
            
            _nonQueryCalls[storedProcedureName]++;
            
            return _nonQueryResponses.ContainsKey(key) ? _nonQueryResponses[key] : 0;
        }

        private string GetKey(string storedProcedureName, SqlParameter[] parameters)
        {
            return storedProcedureName + string.Join("", parameters.Select(p => $"{p.ParameterName}={p.Value}"));
        }
        
        public void VerifyExecuteReader(string storedProcedureName, Times times)
        {
            int expectedCallCount = GetExpectedCallCount(times);
            int actualCallCount = _readerCalls.ContainsKey(storedProcedureName) ? _readerCalls[storedProcedureName] : 0;
            
            if (actualCallCount != expectedCallCount)
            {
                throw new Xunit.Sdk.XunitException($"Expected ExecuteReader to be called {expectedCallCount} times with '{storedProcedureName}', but it was called {actualCallCount} times.");
            }
        }

        public void VerifyExecuteScalar(string storedProcedureName, Times times)
        {
            int expectedCallCount = GetExpectedCallCount(times);
            int actualCallCount = _scalarCalls.ContainsKey(storedProcedureName) ? _scalarCalls[storedProcedureName] : 0;
            
            if (actualCallCount != expectedCallCount)
            {
                throw new Xunit.Sdk.XunitException($"Expected ExecuteScalar to be called {expectedCallCount} times with '{storedProcedureName}', but it was called {actualCallCount} times.");
            }
        }

        public void VerifyExecuteNonQuery(string storedProcedureName, Times times)
        {
            int expectedCallCount = GetExpectedCallCount(times);
            int actualCallCount = _nonQueryCalls.ContainsKey(storedProcedureName) ? _nonQueryCalls[storedProcedureName] : 0;
            
            if (actualCallCount != expectedCallCount)
            {
                throw new Xunit.Sdk.XunitException($"Expected ExecuteNonQuery to be called {expectedCallCount} times with '{storedProcedureName}', but it was called {actualCallCount} times.");
            }
        }
        
        private int GetExpectedCallCount(Times times)
        {
            return times switch
            {
                Times.Never => 0,
                Times.Once => 1,
                Times.Twice => 2,
                _ => throw new ArgumentException("Unsupported Times value")
            };
        }
    }
    
    public enum Times
    {
        Never,
        Once,
        Twice,
        AtLeastOnce,
        AtMostOnce
    }
} 