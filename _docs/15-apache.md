# Apache

## Apache Ignite

Apache Ignite is a distributed database for high-performance computing with in-memory speed.

Is Ignite A Distributed Cache? Yes
When Ignite native persistence is disabled, Ignite can function as a distributed in-memory cache with support distributed ACID transactions, SQL queries, high-performance computing APIs, and more.

Is Ignite A Distributed Database? Yes
Data in Ignite is stored in-memory and/or on-disk, and is either partitioned or replicated across a cluster of multiple nodes. This provides scalability, performance, and resiliency.

Is Ignite An In-Memory Database? Yes
Ignite multi-tier storage supports both in-memory and disk tiers. You can always disable the native persistence and use Ignite as a distributed in-memory database, with support for SQL, transactions and other APIs.

Is Ignite An In-Memory Data Grid? Yes
Ignite is a full-featured distributed data grid. As a grid, Ignite can automatically integrate with and accelerate any 3rd party databases, including any RDBMS or NoSQL stores.


Is Ignite An SQL Database? Not fully
Although Ignite supports SQL natively, there are differences in how Ignite handles constraints and indexes.
Ignite supports primary and secondary indexes, however the uniqueness can only be enforced for the primary indexes. 
Ignite also does not support foreign key constraints at the moment.


Is Ignite A Disk- Or Memory-Only Storage? Both
Native persistence in Ignite can be turned on and off. This allows Ignite to store data sets bigger than can fit in the available memory.
Essentially, smaller operational data sets can be stored in-memory only, and larger data sets that do not fit in memory can be stored on disk, using memory as a caching layer for better performance.

Is Ignite A NoSQL Database? Not exactly
Just like other NoSQL databases, Ignite is highly available and horizontally scalable.
However, unlike other NoSQL databases, Ignite supports SQL and ACID transactions across multiple cluster nodes.

Is Ignite A Transactional Database? Not fully
ACID Transactions are supported, but only at key-value API level. Ignite also supports cross-partition transactions, which means that transactions can span keys residing in different partitions on different servers.

At SQL level, Ignite supports atomic but not yet transactional consistency. A SQL transactions implementation is already in the works and will be released in Ignite 3.

Is Ignite A Multi-Model Database? Yes
Ignite supports both key-value and SQL for modelling and accessing data.
In addition, Ignite provides strong processing APIs for computing on distributed data.

Is Ignite A Key-Value Store? Yes
Ignite provides a feature-rich key-value API that is JCache (JSR-107) compliant and supports Java, C++, .NET, and other programming languages.


## Apache Arrow

Apache Arrow is a cross-language development platform for in-memory data. It specifies a standardized language-independent columnar memory format for flat and hierarchical data, organized for efficient analytic operations on modern hardware.

## Apache Kafka

.NET: Code Example for Apache Kafka

Apache Kafka is a community distributed event streaming platform capable of handling trillions of events a day. 
Initially conceived as a messaging queue, Kafka is based on an abstraction of a distributed commit log. 
Since being created and open sourced by LinkedIn in 2011, Kafka has quickly evolved from messaging queue to a full-fledged event streaming platform.

Founded by the original developers of Apache Kafka, Confluent delivers the most complete distribution of Kafka with Confluent Platform. Confluent Platform improves Kafka with additional community and commercial features designed to enhance the streaming experience of both operators and developers in production, at massive scale.

What are Apache Kafka and RabbitMQ?

Apache Kafka and RabbitMQ are two open-source and commercially-supported pub/sub systems, readily adopted by enterprises. RabbitMQ is an older tool released in 2007 and was a primary component in messaging and SOA systems. 
Today it is also being used for streaming use cases. 
Kafka is a newer tool, released in 2011, which from the onset was built for streaming scenarios.

RabbitMQ is a general purpose message broker that supports protocols including MQTT, AMQP, and STOMP. 
It can deal with high-throughput use cases, such as online payment processing. 
It can handle background jobs or act as a message broker between microservices.

Kafka is a message bus developed for high-ingress data replay and streams. 
Kafka is a durable message broker that enables applications to process, persist, and re-process streamed data. 
Kafka has a straightforward routing approach that uses a routing key to send messages to a topic.

Kafka vs RabbitMQ – Differences in Architecture

RabbitMQ Architecture

1.  General purpose message broker—uses variations of request/reply, point to point, and pub-sub communication patterns.
2.  Smart broker / dumb consumer model—consistent delivery of messages to consumers, at around the same speed as the broker monitors the consumer state.
3.  Mature platform—well supported, available for Java, client libraries, .NET, Ruby, node.js. Offers dozens of plugins.
4.  Communication—can be synchronous or asynchronous.
5.  Deployment scenarios—provides distributed deployment scenarios.
6.  Multi-node cluster to cluster federation—does not rely on external services, however, specific cluster formation plugins can use DNS, APIs, Consul, etc.

Apache Kafka Architecture

1.  High volume publish-subscribe messages and streams platform—durable, fast, and scalable.
2.  Durable message store—like a log, run in a server cluster, which keeps streams of records in topics (categories).
3.  Messages—made up of a value, a key, and a timestamp.
4.  Dumb broker / smart consumer model—does not try to track which messages are read by consumers and only keeps unread messages. Kafka keeps all messages for a set period of time.
5.  Requires external services to run—in some cases Apache Zookeeper.

Pull vs Push Approach

Apache Kafka: Pull-based approach

Kafka uses a pull model. 
Consumers request batches of messages from a specific offset. 
Kafka permits long-pooling, which prevents tight loops when there is no message past the offset.

A pull model is logical for Kafka because of its partitions. 
Kafka provides message order in a partition with no contending consumers. 
This allows users to leverage the batching of messages for effective message delivery and higher throughput.

RabbitMQ: Push-based approach

RabbitMQ uses a push model and stops overwhelming consumers through a prefetch limit defined on the consumer. 
This can be used for low latency messaging.

The aim of the push model is to distribute messages individually and quickly, to ensure that work is parallelized evenly and that messages are processed approximately in the order in which they arrived in the queue.

Feature                 Apache Kafka        RabbitMq
Message ordering        Yes                 N/A
Message lifetime        Always              Until message dequeue
Delivery guarantee      In a partition      No guarantee
Message priority        N/A                 Yes

Apache Kafka:
Kafka offers much higher performance than message brokers like RabbitMQ. 
It uses sequential disk I/O to boost performance, making it a suitable option for implementing queues. 
It can achieve high throughput (millions of messages per second) with limited resources, a necessity for big data use cases.

RabbitMQ:
RabbitMQ can also process a million messages per second but requires more resources (around 30 nodes). 
You can use RabbitMQ for many of the same use cases as Kafka, but you’ll need to combine it with other tools like Apache Cassandra.

Apache Kafka Use Cases

Apache Kafka provides the broker itself and has been designed towards stream processing scenarios. 
Recently, it has added Kafka Streams, a client library for building applications and microservices. 
Apache Kafka supports use cases such as metrics, activity tracking, log aggregation, stream processing, commit logs, and event sourcing.

The following messaging scenarios are especially suited for Kafka:

Streams with complex routing, throughput of 100K/sec events or more, with “at least once” partitioned ordering.
Applications requiring a stream history, delivered in “at least once” partitioned ordering. 
Clients can see a “replay” of the event stream.
Event sourcing, modeling changes to a system as a sequence of events.
Stream processing data in multi-stage pipelines. The pipelines generate graphs of real-time data flows.

RabbitMQ Use Cases

RabbitMQ can be used when web servers need to quickly respond to requests. This eliminates the need to perform resource-intensive activities while the user waits for a result. RabbitMQ is also used to convey a message to various recipients for consumption or to share loads between workers under high load (20K+ messages/second).

Scenarios that RabbitMQ can be used for:

Applications that need to support legacy protocols, such as STOMP, MQTT, AMQP, 0-9-1.
Granular control over consistency/set of guarantees on a per-message basis
Complex routing to consumers


Applications that need a variety of publish/subscribe, point-to-point request/reply messaging capabilities.

Zookeeper is a top-level software developed by Apache that acts as a centralized service and is used to maintain naming and configuration data and to provide flexible and robust synchronization within distributed systems. Zookeeper keeps track of status of the Kafka cluster nodes and it also keeps track of Kafka topics, partitions etc. 

The metadata of Kafka cluster processes is stored in an independent system called Apache Zookeeper. Zookeeper helps Kafka perform several critical functions, such as electing a leader in case of node failure. It also maintains the list of consumers in a consumer group and manages the access control list of Kafka topics.

https://thecloudblog.net/post/event-driven-architecture-with-apache-kafka-for-net-developers-part-1-event-producer/


# Apache Geode

https://gemfire.docs.pivotal.io/apidocs/tgfnc-dotnet-91/a00297.html

Apache Geode provides a database-like consistency model, reliable transaction processing and a shared-nothing architecture to maintain very low latency performance with high concurrency processing.

Apache Geode is a data management platform that provides real-time, consistent access to data-intensive applications throughout widely distributed cloud architectures.

Geode pools memory, CPU, network resources, and optionally local disk across multiple processes to manage application objects and behavior. It uses dynamic replication and data partitioning techniques to implement high availability, improved performance, scalability, and fault tolerance. In addition to being a distributed data container, Apache Geode is an in-memory data management system that provides reliable asynchronous event notifications and guaranteed message delivery.

Apache Geode is a mature, robust technology originally developed by GemStone Systems. 
Commercially available as GemFire™, it was first deployed in the financial sector as the transactional, low-latency data engine used in Wall Street trading platforms. 
Today Apache Geode technology is used by hundreds of enterprise customers for high-scale business applications that must meet low latency and 24x7 availability requirements.

# Apache Flume

Flume is a distributed, reliable, and available service for efficiently collecting, aggregating, and moving large amounts of log data. It has a simple and flexible architecture based on streaming data flows. It is robust and fault tolerant with tunable reliability mechanisms and many failover and recovery mechanisms. It uses a simple extensible data model that allows for online analytic application.

Flume (splunk?)

# Apache Avro

Avro is a row-oriented remote procedure call and data serialization framework developed within Apache's Hadoop project. It uses JSON for defining data types and protocols, and serializes data in a compact binary format.

Microsoft.Hadoop.Avro2

# Apache Airflow

Apache Airflow is an open-source workflow management platform for data engineering pipelines. It started at Airbnb in October 2014 as a solution to manage the company's increasingly complex workflows

https://azkaban.github.io/
https://oozie.apache.org/


# Apache Flink

Apache Flink is an open-source, unified stream-processing and batch-processing framework developed by the Apache Software Foundation. The core of Apache Flink is a distributed streaming data-flow engine written in Java and Scala. Flink executes arbitrary dataflow programs in a data-parallel and pipelined manner


# Apache Pulsar

Apache Pulsar is a cloud-native, distributed messaging and streaming platform originally created at Yahoo! and now a top-level Apache Software Foundation 

# Keycloak

Open Source Identity and Access Management
Add authentication to applications and secure services with minimum effort.
No need to deal with storing users or authenticating users.

Keycloak provides user federation, strong authentication, user management, fine-grained authorization, and mo

https://www.keycloak.org/


# OpenLayers

A high-performance, feature-packed library for all your mapping needs.

https://openlayers.org/


