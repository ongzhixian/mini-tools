# Kafka

    Kafka Cluster—a collection of one or more servers known as brokers
    Producer – the component that is used to publish messages
    Consumer – the component that is used to retrieve or consume messages
    ZooKeeper – a centralized coordination service used to maintain configuration information across cluster nodes in a distributed environment


The latest kafka 3.0 does not work on Windows :-(

Need to use 2.8.1

# Windows Quick Start

Assume installed in `C:\Apps\kafka`


## Start the Kafka environment

2 Steps
1.  Start Zookeeper
2.  Start Kafka broker

# Start the ZooKeeper service (Soon ZooKeeper will no longer be required by Apache Kafka. Yay!)
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties

# Start the Kafka broker service
.\bin\windows\kafka-server-start.bat .\config\server.properties

## Create a topic to store your events

Create topic
.\bin\windows\kafka-topics.bat --create --partitions 1 --replication-factor 1 --topic quickstart-events --bootstrap-server localhost:9092


View topic info
.\bin\windows\kafka-topics.bat --describe --topic quickstart-events --bootstrap-server localhost:9092

Add event
.\bin\windows\kafka-console-producer.bat --topic quickstart-events --bootstrap-server localhost:9092

Read events
.\bin\windows\kafka-console-consumer.bat --topic quickstart-events --from-beginning --bootstrap-server localhost:9092
 
 # C# Package

 dotnet add package Confluent.Kafka