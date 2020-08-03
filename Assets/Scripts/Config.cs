using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Service
{
    /// <summary>
    /// Данный класс является служебным, пожалуйста
    /// не используйте его без необходимости 
    /// </summary>
    public class Config
    {
        public double m { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double maxThrust { get; set; }
        public threeValues sMidelya { get; set; }
        public threeValues centrVolume { get; set; }
        public threeValues hydrodynamiСoef { get; set; }
        public threeValues inercApparat { get; set; }
        public threeValues posEngine1 { get; set; }
        public threeValues posEngine2 { get; set; }
        public threeValues posEngine3 { get; set; }
        public threeValues posEngine4 { get; set; }
    }

    /// <summary>
    /// Данный класс является служебным, пожалуйста
    /// не используйте его без необходимости
    /// </summary>
    public class threeValues
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }
}