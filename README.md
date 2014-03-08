# What is it
NPatterns is a collection of patterns often used in .NET projects, such as patterns of eaa.

## How to get it
* from github
* from nuget: by searching the Name "NPatterns"

## Assemblies
* NPatterns: defines the contracts of patterns, like interface, base class, and basic implementations without any other dependencies.
* NPatterns.XXX: implement some patterns with specified dependencies.

# Patterns implemented:
## MessageBus (a.k.a EventBus)
NPatterns.Messaging.MessageBus, NPatterns

### 1. without Hanlder Factory

#### 1.1. define a message
    public class UserCreatedEvent //UserName,Email etc;

#### 1.2. initial a bus
you could keep it as Singleton, so it could handle all messages in your application via registered handlers

    public static IMessageBus Bus = new MessageBus();

#### 1.3. register handler
you may keep the disposer if the disposing needed sometime

    var disposer = Bus.Subscribe<UserCreatedEvent>(m=>{
        //log for m.UserName
        //send welcome mail to m.Email
    });

#### 1.4. publish message
then registered handler of previous step would be invoked

    var aMessage=new UserCreatedEvent{UserName="abc",Email="abc@abc.com"};
    Bus.Publish<UserCreatedEvent>(aMessage);

### 2. with Hanlder Factory

#### 2.1. define a message (as same as above)

#### 2.2. initial a bus with handler factory    
    kernel.Bind<IMessageBus>() //Ninject
	  .ToConstructor(ctorArg => new MessageBus(type => ctorArg.Context.Kernel.GetAll(type))) //setup handler factory
	  .InSingletonScope(); //make it singleton

#### 2.3. implement a Hanlder
    public class UserCreatedEventHandler:IHandler<UserCreatedEvent>
    {
        public void Handle(UserCreatedEvent message)
        {
            //log for message.UserName
            //send welcome mail to message.Email
        }
    }

#### 2.4. IoC configuration
    kernel.Bind<IHandler<UserCreatedEvent>>().To<UserCreatedEventHandler>(); //Ninject

#### 2.5. publish message (as same as above)

## UnitOfWork
//
## Repository
//
## QueryObject
//