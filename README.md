#S3Archive

A simple application that takes a list of folders and uploads them to S3, optionally ignoring open files, deleting and recursive searching.

Runs as a console application with a single XML file for config.  Designed for cloud servers with small local storage but a regulatory requirement to keep logs.  Run as a scheduled task to collect into an S3 bucket and interrogate your logs centrally.

**NB**: This application uploads data to S3, make sure you are aware of the cost implications and manage your own data retention policies.

##Download binary

<a href="http://itinsurrey.co.uk/assets/files/S3Archive.zip">Download S3Archive</a>

Extract to a folder somewhere and follow instructions.

##Instructions

Run the file S3Archive.exe once, this will create a default config file and exit.  Modify the xml file to include one `folder` element for each path you wish to send to S3.

###Fields/Attributes

  * `includeOpen` - Will also upload open files, defaults to false as webservers usually have an open file handle on the current log.
  * `deleteOnUpload` - Deletes file after it has been uploaded, there is currently no verification so use with caution.
  * `recursive` - Process all subdirectories or just the current.
  * `path` - The local path to the directory to scan.
  * `bucket` - The S3 bucket to store in.  Must already be created, currently this is not created for you.
  * `basePath` - a prepended path to the S3 key for the file to allow you to use a single bucket for multiple sources.
  * `pattern` - File search pattern to use across all included directories


##License

<a rel="license" href="http://creativecommons.org/licenses/by-sa/3.0/"><img alt="Creative Commons Licence" style="border-width:0" src="http://i.creativecommons.org/l/by-sa/3.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" href="http://purl.org/dc/dcmitype/InteractiveResource" property="dct:title" rel="dct:type">S3Archive</span> by <a xmlns:cc="http://creativecommons.org/ns#" href="http://dixcart.com/it" property="cc:attributionName" rel="cc:attributionURL">Dixcart Technical Solutions Limited</a> is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-sa/3.0/">Creative Commons Attribution-ShareAlike 3.0 Unported License</a>.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
