//using Microsoft.Azure.Cosmos.Table;
//using System;
//using System.Collections.Generic;
//using Core.Domain;
//
//namespace CoreBot.Domain
//{
//    public class Team
//    {
//        public Team (string id, string name, int pinCode, UserId leader)
//        {
//            Id = id ?? throw new ArgumentNullException(nameof(id));
//            Name = name ?? throw new ArgumentNullException(nameof(name));
//            Leader = leader ?? throw new ArgumentNullException(nameof(leader));
//            PinCode = pinCode;
//        }
//
//        public string Id { get; }
//        public string Name { get; }
//        public UserId Leader { get; }
//        public int PinCode { get; }
//    }
//}
