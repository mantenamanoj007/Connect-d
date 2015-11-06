from flask import Flask,redirect,request#,session
import json
import random
from flask.ext.mysqldb import MySQL
app=Flask('MS_Hack')
mysql=MySQL()

app.config['MYSQL_USER'] = 'root'
app.config['MYSQL_PASSWORD'] = 'narcos'
app.config['MYSQL_DB'] = 'mshack'
app.config['MYSQL_HOST'] = 'localhost'
app.config['MYSQL_PORT']=3306
mysql.init_app(app)
gid=""

@app.route('/',methods=['GET','POST'])
def index():
    cur = mysql.connection.cursor()
    cur.execute("select * from leader;")
    rv = cur.fetchall()
    print str(rv)
    return 'Hello World'


# @app.route('/val')
# def insert():
#     cur=mysql.connection.cursor()
#     cur.execute("insert into member values(%s,%s,%s,%s,%s)",(1,'name','123.4','123.4',0))
#     mysql.connection.commit()
#     return "Done"


@app.route('/creategroup',methods=['POST'])
def create():
    #Getting JSON data
    jsonf = request.get_json(force=True)
    jdata = json.dumps(jsonf)
    data = json.loads(jdata)
    print data
    print data['username']
    global gid
    gid = str(random.randint(1000,9999))
    print "Generated ",gid

    #Inserting data in Database
    cur=mysql.connection.cursor()
    cur.execute("insert into leader values(%s,%s,%s,%s,%s,0,%s)",(gid,data['username'],data['lon'],data['lat'],data['rad'],data['accurate']))
    mysql.connection.commit()
    return 'Ok'


@app.route('/getgroupid')
def groupid():
    global gid
    return gid


@app.route('/getmemberslocation',methods=['GET'])
def getmembers():
    groupid=request.args.get('groupid')
    print 'Getting members for:'
    # groupid=int(groupid)
    print groupid
    cur=mysql.connection.cursor()
    if groupid is not None:
        cur.execute("select muname,lon,lat,accuracy from member where group_id=" + groupid)
        rv = cur.fetchall()
    res=""
    data={}
    ls=[]
    for i in rv:
        tdata={}
        tdata['username']=i[0]
        tdata['lon']=i[1]
        tdata['lat']=i[2]
        tdata['accuracy']=i[3]
        ls.append(tdata)
    data['data']=ls
    print json.dumps(data)
    return json.dumps(data)


@app.route('/updatemember',methods=['POST'])
def member():
    jsonf = request.get_json(force=True)
    groupid=request.args.get('gid')
    jdata = json.dumps(jsonf)
    data = json.loads(jdata)
    print data
    cur=mysql.connection.cursor()
    cur.execute("update member set lon=%s, lat=%s,accuracy=%s where muname=%s and group_id=%s",(data['longitude'],data['latitude'],data['accuracy'],data['username'],groupid))
    mysql.connection.commit()
    return "Ok"


@app.route('/getleaderlocation',methods=['GET'])
def leaderloc():
    groupid = request.args.get('gid')
    print "Group ID: ",groupid
    cur=mysql.connection.cursor()
    if groupid is not None:
        cur.execute("select luname,lon,lat,accuracy from leader where group_id=" + groupid)
        mysql.connection.commit()
        rv = cur.fetchall()
        res=""
        data={}
        ls=[]
        for i in rv:
            tdata={}
            tdata['username']=i[0]
            tdata['lon']=i[1]
            tdata['lat']=i[2]
            tdata['accuracy']=i[3]
            ls.append(tdata)
        data['data']=ls
        print json.dumps(data)
        return json.dumps(data)

    else:
        print "Group ID invalid"
        return "Invalid"


@app.route('/updateleader',methods=['POST'])
def update():
    jsonf = request.get_json(force=True)
    jdata = json.dumps(jsonf)
    data = json.loads(jdata)
    cur=mysql.connection.cursor()
    cur.execute('update leader set lon=%s,lat=%s,accuracy=%s where group_id=%s',(data['longitude'],data['latitude'],data['accuracy'],data['groupid']))
    mysql.connection.commit()
    print data
    return "Ok"


@app.route('/joingroup',methods=['POST'])
def join():
    #Getting JSON data
    jsonf = request.get_json(force=True)
    jdata = json.dumps(jsonf)
    data = json.loads(jdata)
    print data

    #Inserting data in Database
    cur=mysql.connection.cursor()
    cur.execute("insert into member values(%s,%s,%s,%s,1,%s)",(data['gid'],data['username'],data['latitude'],data['longitude'],data['accuracy']))
    mysql.connection.commit()
    return 'Ok'


@app.route('/getradius',methods=['GET'])
def radius():
    groupid = request.args.get('gid')
    cur=mysql.connection.cursor()
    if groupid is not None:
        cur.execute("select radius from leader where group_id="+groupid)
        rv=cur.fetchall()
        if rv[0][0] is not None:#print rv
            return str(rv[0][0])
        else:
            return 'Group ID invalid'
        #return 'Ok'
    else:
        print "Group ID invalid"
        return "Invalid"


@app.route('/endsession',methods=['GET'])
def end():
    groupid=request.args.get('gid')
    cur=mysql.connection.cursor()
    if groupid is not None:
        cur.execute("delete from leader where group_id="+groupid)
        cur.execute("delete from member where group_id="+groupid)
        mysql.connection.commit()
        return "Deleted"
    else:
        print "Group ID Invalid"
        return "Invalid"


@app.route('/endmembersession',methods=['GET'])
def delmem():
    groupid=request.args.get('username')
    cur=mysql.connection.cursor()
    if groupid is not None:
        cur.execute("delete from member where group_id="+groupid)
        mysql.connection.commit()
        return "Deleted"
    else:
        print "Group ID Invalid"
        return "Invalid"


@app.route('/test')
def test():
    return 'Test Page'


if __name__=='__main__':
    app.run(host='0.0.0.0',port=8080,debug=True)
