# Spark

# Downloads

Apache Spark 3.1.2 (for Hadoop 2.7)
https://spark.apache.org/downloads.html
The package you really want to get as of time of writing is: `spark-3.1.2-bin-hadoop2.7.tgz`
You need to get this file specifically!
Note: Latest Spark  is 3.2.0 but .NET Apache Spark does not support it yet.
      Latest Hadoop is 3.3+  but WinUtils (below) does not support it yet 
      (latest WinUtils is v3.0; but does not exists on Spark download).

.NET for Apache Spark v2.0.0
https://github.com/dotnet/spark/releases

Winutils (Windows binaries for Hadoop)
https://github.com/steveloughran/winutils/blob/master/hadoop-2.7.1/bin/winutils.exe
Copy WinUtils into C:\bin\spark\bin.


# Environment variables

```
setx /M HADOOP_HOME C:\bin\spark-3.0.1-bin-hadoop2.7\
setx /M SPARK_HOME C:\bin\spark-3.0.1-bin-hadoop2.7\
setx /M PATH "%PATH%;%HADOOP_HOME%;%SPARK_HOME%bin" 
```

HADOOP_HOME=C:\Apps\spark
SPARK_HOME=C:\Apps\spark
DOTNET_WORKER_DIR=C:\Apps\sparkWorker


# Test

spark-submit --version

Make sure you can run the following:
dotnet, java, spark-shell

# Packages

dotnet add package Microsoft.Spark

# Submitting Spark job

After `dotnet build`
Go to output folder `MiniTools.HostApp\bin\Debug\net6.0`

```Powershell
spark-submit `
--class org.apache.spark.deploy.dotnet.DotnetRunner `
--master local `
D:\src\github\mini-tools\MiniTools.HostApp\bin\Debug\net6.0\microsoft-spark-3-1_2.12-2.0.0.jar `
dotnet D:\src\github\mini-tools\MiniTools.HostApp\bin\Debug\net6.0\MiniTools.HostApp.dll D:\src\github\mini-tools\input.txt
```

```Powershell (specifying temp folder with `--conf spark.local.dir=D:/debug/temp`)
spark-submit `
--class org.apache.spark.deploy.dotnet.DotnetRunner `
--master local --conf spark.local.dir=D:/debug/temp `
D:\src\github\mini-tools\MiniTools.HostApp\bin\Debug\net6.0\microsoft-spark-3-1_2.12-2.0.0.jar `
dotnet D:\src\github\mini-tools\MiniTools.HostApp\bin\Debug\net6.0\MiniTools.HostApp.dll D:\src\github\mini-tools\input.txt
```

Note: When executing the above, you might get the following error message:
```log
22/01/19 16:27:02 INFO ShutdownHookManager: Deleting directory C:\Users\zhixian\AppData\Local\Temp\spark-08d5e5f6-4515-4db1-92b9-41e4118d747b
22/01/19 16:27:02 ERROR ShutdownHookManager: Exception while deleting Spark temp dir: C:\Users\zhixian\AppData\Local\Temp\spark-08d5e5f6-4515-4db1-92b9-41e4118d747b
java.io.IOException: Failed to delete: C:\Users\zhixian\AppData\Local\Temp\spark-08d5e5f6-4515-4db1-92b9-41e4118d747b\userFiles-0caad24f-12eb-41f5-b94b-b0c2bd0deb37\microsoft-spark-3-1_2.12-2.0.0.jar
```

According to this (https://issues.apache.org/jira/browse/SPARK-12216), its not a permission issue.
Its got to do with JVM/Classloader under Windows is not closing Jar files and keeps them open, so it is not possible to delete these Jar files.
The workaround is to write/fix a custom classloader for these jars and accurately close them before deletion in ShutdownHookManager

A workaround is to go to  Spark (`C:\Apps\spark\conf`) directory and add the following lines to `log4j.properties`:

in your spark directory there is a conf directory - add those two lines to the "log4j.properties" file. 

```ini
log4j.logger.org.apache.spark.util.ShutdownHookManager=OFF
log4j.logger.org.apache.spark.SparkEnv=ERROR
```

If there is no "log4j.properties" there should be a "log4j.properties.template".
Copy the .template and remove the ".template" then add those lines at the top and the error will be hidden.

This does mean that you need to clean `C:\Users\zhixian\AppData\Local\Temp` every so often. :-(


Another workaround for this, instead of letting spark's ShutdownHookManager to delete the temporary directories 
you can issue windows commands to do that,
1.  Change the temp directory using spark.local.dir in spark-defaults.conf file
    `spark.local.dir=D:/debug/temp`
    (or use `--conf spark.local.dir` option)
2.  Set log4j.logger.org.apache.spark.util.ShutdownHookManager=OFF in log4j.properties file
3.  spark-shell internally calls spark-shell.cmd file. So add rmdir /q /s "your_dir\tmp"

I do not like step 3. 
It maybe better to clean the folder manually from time-to-time.


Some ssuggest doing the following; does not work
`winutils.exe chmod 777 D:\debug\temp`