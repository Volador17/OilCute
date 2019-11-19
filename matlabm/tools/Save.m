function Save(d,path,name)
    save(path,'d');
    cmd = sprintf('%s = d; save %s %s',name,path,name);
    eval(cmd);
end