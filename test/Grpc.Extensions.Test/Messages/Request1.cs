using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Test.Messages
{
    public class Request1 : IMessage<Request1>
    {
        public static MessageParser<Request1> Parser { get; } = new MessageParser<Request1>(() => new Request1());
        public string Message { get; set; }

        public MessageDescriptor Descriptor => throw new NotImplementedException();

        public int CalculateSize()
        {
            throw new NotImplementedException();
        }

        public Request1 Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Request1 other)
        {
            throw new NotImplementedException();
        }

        public void MergeFrom(Request1 message)
        {
            throw new NotImplementedException();
        }

        public void MergeFrom(CodedInputStream input)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(CodedOutputStream output)
        {
            throw new NotImplementedException();
        }
    }
}
