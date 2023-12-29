using Main.Tools;

namespace Main.Solutions
{
    internal class Day20Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            (Broadcaster broadcaster, Dictionary<string, Node> nodes) = ProcessLines(inputData);

            long highPulsesSent = 0L;
            long lowPulsesSent = 0L;

            for (int i = 0; i < 1000; i++)
            {
                (long highPulsesSentOnPress, long lowPulsesSentOnPress) = PressButton(broadcaster, nodes);
                highPulsesSent += highPulsesSentOnPress;
                lowPulsesSent += lowPulsesSentOnPress;
            }

            return (highPulsesSent * lowPulsesSent).ToString();
        }

        public string RunPartB(string[] inputData)
        {
            throw new NotImplementedException();
        }

        public static (long, long) PressButton(Broadcaster broadcaster, Dictionary<string, Node> nodes)
        {
            long highPulsesSent = 0L;
            long lowPulsesSent = 0L;

            Queue<Pulse> pulseQueue = new();

            lowPulsesSent++;

            foreach (string destination in broadcaster.Destinations)
            {
                pulseQueue.Enqueue(new Pulse(PulseType.Low, "broadcaster", destination));
                lowPulsesSent++;
            }

            while (pulseQueue.Count > 0)
            {
                Pulse currentPulse = pulseQueue.Dequeue();

                if (nodes.TryGetValue(currentPulse.Receiver, out Node? value))
                {
                    Node currentNode = value;

                    List<Pulse> pulses = currentNode.ReceiveAndSendPulse(currentPulse);

                    foreach (Pulse pulse in pulses)
                    {
                        if (pulse.Type == PulseType.Low)
                        {
                            lowPulsesSent++;
                        }
                        else
                        {
                            highPulsesSent++;
                        }

                        pulseQueue.Enqueue(pulse);
                    }
                }
            }

            return (highPulsesSent, lowPulsesSent);
        }

        public (Broadcaster, Dictionary<string, Node>) ProcessLines(string[] lines)
        {
            Dictionary<string, Node> nodes = [];
            Broadcaster? broadcaster = null;

            foreach (string line in lines)
            {
                string[] parts = line.Split("->", StringSplitOptions.TrimEntries);

                if (parts[0] == "broadcaster")
                {
                    string[] destinations = parts[1].Split(",", StringSplitOptions.TrimEntries);

                    broadcaster = new Broadcaster();

                    foreach (string destination in destinations)
                    {
                        broadcaster.AddDestination(destination);
                    }
                }
                else if (parts[0].StartsWith("%"))
                {
                    Flipflop newFlipFlop = new(parts[0][1..]);

                    string[] destinations = parts[1].Split(",", StringSplitOptions.TrimEntries);

                    foreach (string destination in destinations)
                    {
                        newFlipFlop.AddDestination(destination);
                    }

                    nodes.Add(newFlipFlop.Name, newFlipFlop);
                }
                else if (parts[0].StartsWith("&"))
                {
                    Converter newConverter = new(parts[0][1..]);

                    string[] destinations = parts[1].Split(",", StringSplitOptions.TrimEntries);

                    foreach (string destination in destinations)
                    {
                        newConverter.AddDestination(destination);
                    }

                    nodes.Add(newConverter.Name, newConverter);
                }
            }

            foreach (Converter converter in nodes.Values.OfType<Converter>())
            {
                foreach (Node sender in nodes.Values.Where(sender => sender.Destinations.Contains(converter.Name)))
                {
                    converter.AddReceivingNode(sender.Name);
                }

                if (broadcaster != null && broadcaster.Destinations.Contains(converter.Name))
                {
                    converter.AddReceivingNode("broadcaster");
                }
            }

            if (broadcaster != null)
            {
                return (broadcaster, nodes);
            }

            throw new Exception("Broadcaster not found.");
        }
    }

    internal enum PulseType
    {
        Low,
        High
    }

    internal class Pulse(PulseType type, string emmiter, string receiver)
    {
        public PulseType Type = type;
        public string Emmiter = emmiter;
        public string Receiver = receiver;
    }

    internal abstract class Node
    {
        public List<string> Destinations = [];
        public string Name;

        public Node(string name)
        {
            this.Name = name;
        }

        public abstract List<Pulse> ReceiveAndSendPulse(Pulse pulse);
        public void AddDestination(string destinationName)
        {
            Destinations.Add(destinationName);
        }
    }

    internal class Flipflop : Node
    {
        public bool isHigh;
        public Flipflop(string name) : base(name)
        {
            this.isHigh = false;
        }

        public override List<Pulse> ReceiveAndSendPulse(Pulse pulse)
        {
            List<Pulse> listOfPulses = [];

            if (pulse.Type == PulseType.Low)
            {
                this.isHigh = !this.isHigh;

                foreach (string destination in this.Destinations)
                {
                    listOfPulses.Add(new Pulse(this.isHigh ? PulseType.High : PulseType.Low, this.Name, destination));
                }
            }

            return listOfPulses;
        }
    }

    internal class Converter : Node
    {
        public Converter(string name) : base(name)
        {
        }

        public Dictionary<string, PulseType> ReceivingPulses = [];

        public void AddReceivingNode(string name)
        {
            ReceivingPulses.Add(name, PulseType.Low);
        }

        public override List<Pulse> ReceiveAndSendPulse(Pulse pulse)
        {
            List<Pulse> listOfPulses = [];

            if (this.ReceivingPulses.ContainsKey(pulse.Emmiter))
            {
                this.ReceivingPulses[pulse.Emmiter] = pulse.Type;

                PulseType pulseToSend = this.ReceivingPulses.Values.All(type => type == PulseType.High) ?
                    PulseType.Low :
                    PulseType.High;

                foreach (string destination in this.Destinations)
                {
                    listOfPulses.Add(new Pulse(pulseToSend, this.Name, destination));
                }

                return listOfPulses;
            }
            else
            {
                throw new Exception("Node not found in Converter");
            }
        }
    }

    internal class Broadcaster
    {
        public List<string> Destinations = [];

        public void AddDestination(string name)
        {
            Destinations.Add(name);
        }
    }
}
