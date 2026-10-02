using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace InteriorArchitect2D.Models
{
    [JsonDerivedType(typeof(Wall), typeDiscriminator: "Wall")]
    [JsonDerivedType(typeof(Furniture), typeDiscriminator: "Furniture")]
    public abstract class ProjectElement
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public double X { get; set; }
        public double Y { get; set; }

        public int Layer { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; } = false;

        public abstract void Draw(Canvas canvas, double scale);

        public virtual void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }
    }
}