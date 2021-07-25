using BenchmarkDotNet.Running;
using JsonSerializerCodeGen;

BenchmarkRunner.Run<DeserializeColdStart>();
BenchmarkRunner.Run<Deserialize>();
