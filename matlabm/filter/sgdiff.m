%采用Savitzky-Golay方法进行求导
%X：吸光度矩阵；m：窗口宽度，必须为奇数，必须大于3；p：导数阶数;采用3次多项式。
%d：一阶微分后的吸光度矩阵。
%主要参考文件：基础化学计量学（科学出版社），P47
function d=sgdiff(X,m,p);

	k=2;%采用三次多项式

	% 计算D矩阵
	s = fliplr(vander(-(m-1)./2:(m-1)./2));
	S = s(:,1:k+1);   
	D1=S'*S;
	D2=inv(D1);
	D=D2*S';

	[yr,yc]=size(X);
	mm=(m+1)/2;
	mmm=(m-1)/2;

	%两端补零
	d=zeros(yr,yc);

	%计算导数光谱
	pp=1;
	for ii=1:p
		pp=pp*ii;
	end %导数阶数的阶层
	pp=pp*ones(1,yc);
	for i=mm:yr-mmm
		xx=X((i-mmm):(i+mmm),:);
		A=D*xx;
		d(i,:)=A(p+1,:).*pp;
	end
end
