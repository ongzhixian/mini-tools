# Orleans


Microsoft.Orleans.Server
Microsoft.Orleans.Client

Microsoft.Orleans.Core.Abstractions
Microsoft.Orleans.CodeGenerator.MSBuild

Microsoft.Extensions.Logging.Console
Microsoft.Extensions.Logging.Abstractions


Silo 	            Microsoft.Orleans.Server
Silo 	            Microsoft.Extensions.Logging.Console

Client 	            Microsoft.Extensions.Logging.Console
Client 	            Microsoft.Orleans.Client

Grain Interfaces 	Microsoft.Orleans.Core.Abstractions
Grain Interfaces 	Microsoft.Orleans.CodeGenerator.MSBuild

Grains 	            Microsoft.Orleans.CodeGenerator.MSBuild
Grains 	            Microsoft.Orleans.Core.Abstractions
Grains 	            Microsoft.Extensions.Logging.Abstractions

## Concepts

Grain

The fundamental building block in any Orleans application is a grain. 
Grains are entities comprising:

1.  user-defined identity, 
2.  behavior, and 
3.  state. 

Grain identities are user-defined keys which make Grains always available for 
invocation. Grains can be invoked by other grains or by external clients such as Web frontends, via strongly-typed communication interfaces (contracts). 
Each grain is an instance of a class which implements one or more of these interfaces.

Grains can have volatile and/or persistent state that can be stored in any storage system. 
As such, grains implicitly partition application state, enabling automatic scalability and simplifying recovery from failures. 
Grain state is kept in memory while the grain is active, leading to lower latency and less load on data stores.

Consider a cloud backend for an Internet of Things system. 
This application needs to process incoming device data, filter, aggregate, and process this information, and enable sending commands to devices. 

In Orleans, it is natural to model each device with a grain which becomes a digital twin of the physical device it corresponds to. 
These grains keep the latest device data in memory, so that they can be quickly queried and processed without the need to communicate with the physical device directly. 
By observing streams of time-series data from the device, the grain can detect changes in conditions, such as measurements exceeding a threshold, and trigger an action.

Silo / Orleans Runtime

The Orleans runtime is what implements the programming model for applications.
The main component of the runtime is the silo, which is responsible for hosting grains. Typically, a group of silos run as a cluster for scalability and fault-tolerance. 
When run as a cluster, silos coordinate with each other to distribute work, detect and recover from failures. 
The runtime enables grains hosted in the cluster to communicate with each other as if they are within a single process.

In addition to the core programming model, the silo provides grains with a set of runtime services, such as timers, reminders (persistent timers), persistence, transactions, streams, and more. See the features section below for more detail.

Web frontends and other external clients call grains in the cluster using the client library which automatically manages network communication. Clients can also be co-hosted in the same process with silos for simplicity.

Orleans is compatible with .NET Standard 2.0 and above, running on Windows, Linux, and macOS, in full .NET Framework or .NET Core.

# Minimal

Orleans distributed, clustered, HA services.

At minimal we need 4 projects.

Grains  references GrainInterfaces.
Silo    references GrainInterfaces and Grains.
Client  references GrainInterfaces.

(classlib)  GrainInterfaces are (service) contracts.
(classlib)  Grains implements GrainInterfaces (services). Grains = Actors
(console)   Silo hosts grains.
(console)   Client calls grains.

Client connects to a Cluster and get grains.


# Orleans + Kubernetes (complementary)

Re-activation of grains upon host failure is not scaling, it is application reliability (auto-healing) and HA (virtual actors; grains always exists).
Aside: Orleans also have distributed transactions.
Orleans also has scalability (but more focus on grains' (grainular) scalability).

Orleans runtime:
Activates Grain instance on a cluster
Manage the Grain lifecycle; you don’t have to deal with activation or deactivation. 
Instances always exist, virtually
Ensures the execution will be single-threaded
Automatically deactivates idle Grains
Automatically recover on failure
Manages only one instance for stateful Grains
Manages multiple numbers of instances for stateless Grains if needed for the best performance
Grain references can be stored or you can pass to another Grain.


Kubernetes do not have this.
Kubernetes has scales on machine-level (spin up more instance)


# Actor-Model

The actor model adopts the philosophy that everything is an actor. 
This is similar to the everything is an object philosophy used by some object-oriented programming languages.

An actor is a computational entity that, in response to a message it receives, can concurrently:

1.  send a finite number of messages to other actors;
2.  create a finite number of new actors;
3.  designate the behavior to be used for the next message it receives.

There is no assumed sequence to the above actions and they could be carried out in parallel.

Decoupling the sender from communications sent was a fundamental advance of the actor model enabling asynchronous communication and control structures as patterns of passing messages.

Recipients of messages are identified by address, sometimes called "mailing address". 
Thus an actor can only communicate with actors whose addresses it has. 
It can obtain those from a message it receives, or if the address is for an actor it has itself created.

The actor model is characterized by inherent concurrency of computation within and among actors, dynamic creation of actors, inclusion of actor addresses in messages, and interaction only through direct asynchronous message passing with no restriction on message arrival order. 
