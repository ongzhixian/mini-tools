using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MiniTools.HostApp.Services;

internal class KafkaExample
{
    public void DoWork()
    {
        // Config 

        Dictionary<string, string> cloudConfig = new Dictionary<string, string>();
        cloudConfig.Add("bootstrap.servers", "localhost:9092");

        var clientConfig = new ClientConfig(cloudConfig);
        //clientConfig.SslCaLocation = certDir;


        //Produce("quickstart-events", clientConfig);
        //await CreateTopicMaybe(topic, 1, 3, config);

        Consume("quickstart-events", clientConfig);


    }

    void Produce(string topic, ClientConfig config)
    {
        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            int numProduced = 0;
            int numMessages = 10;
            for (int i = 0; i < numMessages; ++i)
            {
                var key = "alice";
                //var val = JObject.FromObject(new { count = i }).ToString(Formatting.None);

                var val = System.Text.Json.JsonSerializer.Serialize(new { count = i });

                Console.WriteLine($"Producing record: {key} {val}");

                producer.Produce(topic, new Message<string, string> { Key = key, Value = val },
                    (deliveryReport) =>
                    {
                        if (deliveryReport.Error.Code != ErrorCode.NoError)
                        {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                            numProduced += 1;
                        }
                    });
            }

            producer.Flush(TimeSpan.FromSeconds(10));

            Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
        }
    }

    static async Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig)
    {
        using (var adminClient = new AdminClientBuilder(cloudConfig).Build())
        {
            try
            {
                await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = name, NumPartitions = numPartitions, ReplicationFactor = replicationFactor } });
            }
            catch (CreateTopicsException e)
            {
                if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    Console.WriteLine($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                }
                else
                {
                    Console.WriteLine("Topic already exists");
                }
            }
        }
    }

    void Consume(string topic, ClientConfig config)
    {
        var consumerConfig = new ConsumerConfig(config);
        consumerConfig.GroupId = "dotnet-example-group-1";
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
        consumerConfig.EnableAutoCommit = false;

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
        {
            consumer.Subscribe(topic);
            var totalCount = 0;
            try
            {
                while (true)
                {
                    var cr = consumer.Consume(cts.Token);
                    //totalCount += JObject.Parse(cr.Message.Value).Value<int>("count");
                    //System.Text.Json.JsonSerializer.Deserialize<string>(cr.Message.Value);
                    //Console.WriteLine($"Consumed record with key {cr.Message.Key} and value {cr.Message.Value}, and updated total count to {totalCount}");
                    Console.WriteLine($"Consumed record with key {cr.Message.Key} and value {cr.Message.Value}");
                }
            }
            catch (OperationCanceledException)
            {
                // Ctrl-C was pressed.
            }
            finally
            {
                consumer.Close();
            }
        }
    }

}
