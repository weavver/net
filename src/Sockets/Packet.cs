using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Weavver.Sockets
{
     public class Packet
     {
          private string packet    = "";
//--------------------------------------------------------------------------------------------
          public Packet(string packet)
          {
               this.packet = packet;
          }
//--------------------------------------------------------------------------------------------
          public string this[string property]
          {
               get
               {

                    return GetValue(property);
               }
               set
               {

               }
          }
//--------------------------------------------------------------------------------------------
          public string Body
          {
               get
               {
                    string temp = packet.Substring(packet.IndexOf("\r\n\r\n") + 4);
                    return temp;
               }
          }
//--------------------------------------------------------------------------------------------
          private string GetValue(string property)
          {
               string[]  line      = Regex.Split(packet, Environment.NewLine);
               string    value     = null;

               for( int i = 0; i < line.Length; i++ )
               {
                    if( line[i].StartsWith(property) )
                    {
                         value = line[i].Substring(line[i].IndexOf(":") + 2);
                         break;
                    }
               }
               return value;
          }
//--------------------------------------------------------------------------------------------
          public override string ToString()
          {
               return packet;               
          }
//--------------------------------------------------------------------------------------------

     }
}
