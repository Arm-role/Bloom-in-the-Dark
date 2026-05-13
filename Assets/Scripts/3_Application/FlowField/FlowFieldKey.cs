using System;
using UnityEngine;

public struct FlowFieldKey : IEquatable<FlowFieldKey>
{
  private FlowFieldChannelKey channel;
  private Vector2Int footprint;


  public FlowFieldKey(FlowFieldChannelKey channel, Vector2Int footprint)
  {
    this.channel = channel;
    this.footprint = footprint;
  }

  public bool Equals(FlowFieldKey other)
  {
    return channel == other.channel &&
           footprint == other.footprint;
  }

  public override bool Equals(object obj)
  {
    return obj is FlowFieldKey other &&
           Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(channel, footprint);
  }
}